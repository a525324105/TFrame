package main

//-----------------------------------
//File:main.go
//Date:2022年2月15日
//Desc:帧同步战斗服务器
//Auth:AlexTang
//-----------------------------------

import (
	"flag"
	"fmt"
	"net/http"
	_ "net/http/pprof"
	"os"
	"os/signal"
	"strings"
	"syscall"
	"time"

	"NetServer/kcp_server"
	"NetServer/protocol"
	"NetServer/room"
	_ "NetServer/web"

	"NetServer/config"

	"NetServer/util"

	"github.com/wonderivan/logger"
)

var (
	nodeId     = flag.Uint64("id", 0, "id")
	configFile = flag.String("config", "config.xml", "config file")
	gWeb       = flag.String("web", ":10002", "web listen address")
	outAddress = flag.String("out", ":10086", "out listen address(':10086' means use $localip:10086)")
)

//LoadConfig 加载配置
func LoadConfig() bool {

	if err := config.LoadConfig(*configFile); err != nil {
		panic(fmt.Sprintf("[main] load config %v fail: %v", *configFile, err))
	}

	config.Cfg.OutAddress = *outAddress
	temp := strings.Split(config.Cfg.OutAddress, ":")
	if 0 == len(temp[0]) {
		config.Cfg.OutAddress = util.GetOutboundIP().String() + config.Cfg.OutAddress
	}

	return true
}

//Init 初始化
func Init() bool {
	if len(*gWeb) > 0 {

		go func() {
			e := http.ListenAndServe(*gWeb, nil)
			if nil != e {
				panic(e)
			}
		}()
		logger.Info("[main] http.ListenAndServe port=[%s]", *gWeb)
	}

	return true
}

//Run 运行
func Run() {

	defer func() {
		//clear
		time.Sleep(time.Millisecond * 100)
		logger.Alert("[main] pvp %d quit", *nodeId)
	}()

	defer room.Stop()

	//udp server
	networkServer, err := kcp_server.ListenAndServe(config.Cfg.OutAddress, &room.Router{}, &protocol.MsgProtocol{})
	if nil != err {
		panic(err)
	}
	logger.Info("[main] kcp.Listen addr=[%s]", config.Cfg.OutAddress)
	defer networkServer.Stop()

	logger.Info("[main] cluster start! etcd=[%s] key=[%s]", config.Cfg.EtcdEndPionts, config.Cfg.EtcdKey)

	//主循环定时器
	ticker := time.NewTimer(time.Second)
	defer ticker.Stop()

	sigs := make(chan os.Signal, 1)
	signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM, syscall.SIGHUP, os.Interrupt)

	logger.Warn("[main] %d running...", *nodeId)
	//主循环
QUIT:
	for {
		select {
		case sig := <-sigs:
			logger.Info("Signal: %s", sig.String())
			if sig == syscall.SIGHUP {

			} else {
				break QUIT
			}
		case <-ticker.C:
			//break QUIT
		}

	}

	logger.Info("[main] pvp %d quiting...", *nodeId)
}

func main() {

	showIP := false
	flag.BoolVar(&showIP, "ip", false, "show ip info")
	flag.Parse()
	if showIP {
		fmt.Println("GetOutboundIP", util.GetOutboundIP())
		fmt.Println("GetLocalIP", util.GetLocalIP())
		fmt.Println("GetExternalIP", util.GetExternalIP())
		os.Exit(0)
	}

	if LoadConfig() && Init() {
		Run()
	} else {
		fmt.Printf("[main] launch fail")
	}
}
