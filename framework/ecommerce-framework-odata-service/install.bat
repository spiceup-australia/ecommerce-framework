prunsrv.exe //IS//SDLWebECommerce --DisplayName="SDL Web E-Commerce" --Description="SDL Web E-Commerce" --Startup=auto --Install=%CD%\prunsrv.exe --Jvm=auto --Classpath=%CD%\target\ecommerce-framework-odata-service-1.1.0.jar --StartMode=jvm --StartClass=com.sdl.ecommerce.odata.service.Bootstrap --StartMethod=start --StartParams=start --StopMode=jvm --StopClass=com.sdl.ecommerce.service.Bootstrap --StopMethod=stop --StopParams=stop --StdOutput=auto --StdError=auto --LogPath=%CD%\logs --LogLevel=Info

