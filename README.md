# Process for UWP
一种适用于UWP平台的Process方法

基于[@Silver-Fang](https://github.com/Silver-Fang "Silver-Fang")的源码([Github](https://github.com/Silver-Fang/ProcessForUWP "ProcessForUWP"))

<a href="https://github.com/wherewhere/Process-For-UWP/blob/master/LICENSE"><img alt="GitHub" src="https://img.shields.io/github/license/wherewhere/Process-For-UWP.svg?label=License&style=flat-square"></a>
<a href="https://github.com/wherewhere/Process-For-UWP/issues"><img alt="GitHub" src="https://img.shields.io/github/issues/wherewhere/Process-For-UWP.svg?label=Issues&style=flat-square"></a>
<a href="https://github.com/wherewhere/Process-For-UWP/stargazers"><img alt="GitHub" src="https://img.shields.io/github/stars/wherewhere/Process-For-UWP.svg?label=Stars&style=flat-square"></a>

<a href="https://github.com/wherewhere/Process-For-UWP/releases/latest"><img alt="GitHub All Releases" src="https://img.shields.io/github/downloads/wherewhere/Process-For-UWP/total.svg?label=DOWNLOAD&logo=github&style=for-the-badge"></a>
<a href=""><img alt="Baidu Netdisk" src="https://img.shields.io/badge/download-%e5%af%86%e7%a0%81%ef%bc%9alIIl-magenta.svg?label=%e4%b8%8b%e8%bd%bd&logo=baidu&style=for-the-badge"></a>

## 目录
- [Process for UWP](#process-for-uwp)
  - [目录](#目录)
  - [如何使用](#如何使用)
  - [Star 数量统计](#star-数量统计)

## 如何使用
在你的解决方案中添加一个打包项目和一个空白桌面应用项目。在打包项目中引用你的UWP项目和桌面应用项目。在UWP项目中添加引用“UWP端.dll”，在桌面应用项目中引用“桌面端.dll”。注意UWP端.dll是依赖桌面端.dll的，所以桌面端.dll也需要添加到你的UWP项目中，但是不需要添加引用。然后在打包项目的Package.appxmanifest中：  
在Package根节点下添加属性：xmlns:desktop="http://schemas.microsoft.com/appx/manifest/desktop/windows10"  
在Package\Applications\Application下添加子节点：  
```xml
<Extensions>
	<desktop:Extension Category="windows.fullTrustProcess" Executable="【桌面应用项目的路径，如：远程代理\远程代理.exe】">
		<desktop:FullTrustProcess>
			<desktop:ParameterGroup GroupId="环回配置" Parameters="【包系列名+空格+TCP环回端口号，如：642671AC6A72D.52333923F7214_9vcz5tcd8ce5e 32768】"  />
		</desktop:FullTrustProcess>
	</desktop:Extension>
</Extensions>
```
在你的空白桌面项目中添加新代码文件，输入以下代码：
```vb
Module 远程代理
	Sub Main()
		ProcessForUWP.桌面端.启动远程代理(Command)
	End Sub
End Module
```
在项目属性中将启动项目设为Sub Main。
在你的UWP项目中需要使用Process类的地方，务必在代码文件中Imports ProcessForUWP.UWP端，否则会变成使用原本的Process类了。其它代码不需要修改。  
在解决方案配置管理器中，三个项目的平台需保持一致，建议都设为x64。生成都要勾选。部署只需勾选打包项目，UWP项目和桌面项目都不需部署。  
解决方案的启动项目应设为打包项目。  
如果你有其它需求也可以尝试在UWP项目中直接调用桌面端DLL，但是此用法未经测试，可用性未知，谨慎使用。  

## Star 数量统计
[![Star 数量统计](https://starchart.cc/wherewhere/ProcessForUWP.svg)](https://starchart.cc/wherewhere/ProcessForUWP "Star 数量统计")
