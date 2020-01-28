# 基于 ESP wifi 模块的 smartconfig 之 esptouch 协议智能配网的 .net 实现
移植于 EsptouchForAndroid 项目


## 说明
1、广播模式、组播方式测试通过、由于 AES 没有使用，所以没有移植  
2、DLL 项目基于 .NET Stdandard 2.0; 命令行测试程序 基于 .NET Core 3.1   
3、解决方案使用 VS2019 构建  

## 注意
1、命令行程序在 windows 下可以自动获取当前连接WIFI的SSID、BSSID、IP; 其他平台可以获取IP，其他参数手动输入  
2、广播模式，可以在不同路由器下完成配网；组播模式，ESP8266 模块必须跟 EspTouchForCSharp 主机再同一个路由器下  
3、广播模式下，不需要运行 EspTouchForCSharp 主机的网络连接模式，不必一定是 WIFI 连接  

## 命令行参数  
```
Usage: EsptouchNetCore ...
  [ --(ssid | s) SSID ]
  [ --(bssid | b) BSSID ]
  [ --(address | a) LOCALADDRESS ]
  [ --(broadcast | br) <1|0> ,default:1 ]
  [ --(devices | d) NUMBER ,default:1 ]
  [ --(password | p) PASSWORD ]
  [ --(help | h) USAGE ]
```
