### MapServer

Google Map Server
#### 快速开始
##### 1. 使用Docker镜像
访问Docker Hub 寻找 [kobatomir/mapserver](https://hub.docker.com/r/kobatomir/mapserver) 查看,amd64版本查看Tag为 amd64的镜像,arm64版本请查看 arm64v8的镜像

dockerfile镜像详见docker文件夹

其微软runtime 见 [aspnetcore]([ASP.NET Core Runtime (docker.com)](https://hub.docker.com/_/microsoft-dotnet-aspnet))

##### 2.本地编译发布
使用 dotnet publish 发布
> dotnet publish -r linux-arm64  发布arm64原生
> docker publish -r linux-x64      发布x64原生

