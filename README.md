# Process for UWP
一种适用于 UWP 平台的 Process 方法

基于 [@Silver-Fang](https://github.com/Silver-Fang "Silver-Fang") 的项目 ([Github](https://github.com/Silver-Fang/ProcessForUWP "ProcessForUWP"))

[![LICENSE](https://img.shields.io/github/license/wherewhere/Process-For-UWP.svg?label=License&style=flat-square)](https://github.com/wherewhere/Process-For-UWP/blob/master/LICENSE "LICENSE")
[![Issues](https://img.shields.io/github/issues/wherewhere/Process-For-UWP.svg?label=Issues&style=flat-square)](https://github.com/wherewhere/Process-For-UWP/issues "Issues")
[![Stargazers](https://img.shields.io/github/stars/wherewhere/Process-For-UWP.svg?label=Stars&style=flat-square)](https://github.com/wherewhere/Process-For-UWP/stargazers "Stargazers")

[![UWP](https://img.shields.io/nuget/dt/ProcessForUWP.UWP.svg?logo=NuGet&style=for-the-badge)](https://www.nuget.org/packages/ProcessForUWP.UWP "UWP")
[![Desktop](https://img.shields.io/nuget/dt/ProcessForUWP.Desktop.svg?logo=NuGet&style=for-the-badge)](https://www.nuget.org/packages/ProcessForUWP.Desktop "Desktop")

## 目录
- [Process for UWP](#process-for-uwp)
  - [目录](#目录)
  - [简介](#简介)
  - [如何使用](#如何使用)
  - [注意事项](#注意事项)
  - [Star 数量统计](#star-数量统计)

## 简介
本项目基于 [@Silver-Fang](https://github.com/Silver-Fang "Silver-Fang") 的项目 [ProcessForUWP](https://github.com/Silver-Fang/ProcessForUWP "ProcessForUWP")，与其不同的是，本项目利用了通信接口 `AppServiceConnection` 来进行应用间的通信，所以不用执行一次就弹一次 UAC，但是使用起来会比较麻烦。目前仍在开发当中，如果有兴趣欢迎加入。

## 如何使用
在你的解决方案中添加一个打包项目和一个空白桌面应用项目。在打包项目中引用你的 UWP 项目和桌面应用项目。在 UWP 项目中添加引用 `ProcessForUWP.UWP`，在桌面应用项目中引用 `ProcessForUWP.Desktop`。 

然后在打包项目的 `Package.appxmanifest` 中添加：
```xml
<Package
    ...
    xmlns:desktop="http://schemas.microsoft.com/appx/manifest/desktop/windows10" 
    IgnorableNamespaces="uap rescap desktop">
    ...
    <Applications>
        <Application>
            ...
            <Extensions>
                <uap:Extension Category="windows.appService">
                    <uap:AppService Name="ProcessForUWP.Delegate"/>
                </uap:Extension>
                <desktop:Extension Category="windows.fullTrustProcess" Executable="【桌面应用项目的路径，如：ProcessForUWP.Demo.Delegate\ProcessForUWP.Demo.Delegate.exe】" />
            </Extensions>
        </Application>
    </Applications>
    ...
</Package>
```
在 UWP 项目的 `App.xaml.cs` 中添加:
```cs
public sealed partial class App : Application
{
    ...
    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
        Communication.InitializeAppServiceConnection();
        ...
    }

    /// <summary>
    /// Handles connection requests to the app service
    /// </summary>
    /// <param name="args">Data about the background activation.</param>
    protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
    {
        base.OnBackgroundActivated(args);
        Communication.OnBackgroundActivated(args);
    }
}
```
在桌面项目的 `Program.cs` 中添加：
```cs
internal class Program
{
    private static void Main(string[] args)
    {
        Communication.InitializeAppServiceConnection();
        ...
        EventWaitHandle WaitHandle = new AutoResetEvent(false);
        _ = WaitHandle.WaitOne();
    }
    ...
}
```
在解决方案配置管理器中，三个项目的平台需保持一致，建议都设为x64。生成都要勾选。部署只需勾选打包项目，UWP 项目和桌面项目都不需部署。  
解决方案的启动项目应设为打包项目。  

## 注意事项
1. 具体使用方法请查看 Demo
2. 请不要在应用初始化过程中新建 Process 类，虽然我已经做了未加载完成的判断，但是这个判断可能会占用了线程导致应用初始化无法完成，如果真的要在应用初始化时新建 Process 类，请把它挪到别的线程去，或者提出 issue 帮助我解决这个问题。

## Star 数量统计
[![Star 数量统计](https://starchart.cc/wherewhere/ProcessForUWP.svg)](https://starchart.cc/wherewhere/ProcessForUWP "Star 数量统计")
