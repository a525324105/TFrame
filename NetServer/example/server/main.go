package main

import (
	"flag"
	"fmt"
	"net/http"
	_ "net/http/pprof"
	"os"
	"os/signal"
	"syscall"
	"time"

	"NetServer/example/server/api/web"
	"NetServer/kcp_server"
	"NetServer/protocol"
	"NetServer/room"
	"NetServer/router"
	"NetServer/util"

	"github.com/wonderivan/logger"
)

var (
	nodeId     = flag.Uint64("id", 0, "id")
	httpAddres = flag.String("web", ":10002", "web listen address")
	udpAddress = flag.String("udp", ":10086", "udp listen address(':10086' means localhost:10086)")
	debugLog   = flag.Bool("log", true, "debug log")
	m          *room.RoomManager
)

// LoadConfig 加载配置
func LoadConfig() bool {
	return true
}

// Init 初始化
func Init() bool {
	// if *debugLog {
	// 	logger.Global.Close()
	// 	logger.AddFilter("debug logger", logger.DEBUG, util.NewColorConsoleLogWriter())
	// }
	m = room.NewRoomManager()

	go func() {
		e := http.ListenAndServe(*httpAddres, nil)
		if nil != e {
			panic(e)
		}
	}()
	logger.Info("[main] http.ListenAndServe port=[%s]", *httpAddres)

	return true
}

//Run 运行
func Run() {

	defer func() {
		//clear
		time.Sleep(time.Millisecond * 100)
		logger.Alert("[main] pvp %d quit", *nodeId)
	}()

	//address := util.GetLocalIP()
	//udp server
	networkServer, err := kcp_server.ListenAndServe(*udpAddress, router.New(m), &protocol.MsgProtocol{})
	if nil != err {
		panic(err)
	}
	logger.Info("[main] kcp.Listen addr=[%s]", *udpAddress)
	defer networkServer.Stop()

	defer m.Stop()

	_ = web.NewWebAPI(m)

	ticker := time.NewTimer(time.Minute)
	defer ticker.Stop()

	sigs := make(chan os.Signal, 1)
	signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM, syscall.SIGHUP, os.Interrupt)

	logger.Warn("[main] running...")
	//主循环
QUIT:
	for {
		select {
		case sig := <-sigs:
			logger.Info("Signal: %s", sig.String())
			if sig == syscall.SIGHUP {
				// reload
			} else {
				break QUIT
			}
		case <-ticker.C:
			// todo
			fmt.Println("room number ", m.RoomNum())
		}

	}

	logger.Info("[main] quiting...")
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
