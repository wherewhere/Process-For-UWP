# Process for UWP
一种适用于 UWP 平台的 Process 方法

曾基于 [@Silver-Fang](https://github.com/Silver-Fang "Silver-Fang") 的项目 ([Github](https://github.com/Silver-Fang/ProcessForUWP "ProcessForUWP"))

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
本项目基于 `OOP/COM` (`Out-of-Process`) 通信机制实现了 UWP 平台下的 `Process` 类，支持大部分常用方法和属性。目前仍在开发当中，如果有兴趣欢迎加入。

## 如何使用
在你的解决方案中添加一个打包项目和一个空白桌面应用项目。在打包项目中引用你的 UWP 项目和桌面应用项目。在 UWP 项目中引用 `ProcessForUWP.UWP`，在桌面应用项目中引用 `ProcessForUWP.Desktop`。 

- OOP/COM 通信

  在打包项目的 `Package.appxmanifest` 中添加：
  ```xml
  <Package
    ...
    xmlns:com="http://schemas.microsoft.com/appx/manifest/com/windows10"
    IgnorableNamespaces="... com">
    ...
    <Applications>
      <Application>
        ...
        <Extensions>
          ...
          <com:Extension Category="windows.comServer">
            <com:ComServer>
              <com:ExeServer
                Executable="【桌面应用项目的路径，如：ProcessForUWP.Demo.Delegate\ProcessForUWP.Demo.Delegate.exe】"
                DisplayName="ProcessForUWP Delegate"
                LaunchAndActivationPermission="O:SYG:SYD:(A;;11;;;WD)(A;;11;;;RC)(A;;11;;;AC)(A;;11;;;AN)S:P(ML;;NX;;;S-1-16-0)">
                <com:Class Id="【自行生成的唯一 GUID】" DisplayName="ProcessForUWP Delegate" />
              </com:ExeServer>
            </com:ComServer>
          </com:Extension>
        </Extensions>
      </Application>
    </Applications>
    ...
  </Package>
  ```

  在桌面项目的 `Program.cs` 中添加：
  ```cs
  internal class Program
  {
      private static void Main(string[] args)
      {
          ...
          Factory.StartComServer(new Guid("【自行生成的唯一 GUID】"));
      }
      ...
  }
  ```

  在 UWP 项目中使用前添加：
  ```cs
  ProcessProjectionFactory.CLSID = new Guid("【自行生成的唯一 GUID】");
  ```

- WinRT/COM 通信 (不支持沙盒与沙盒外通信)

  在打包项目的 `Package.appxmanifest` 中添加：
  ```xml
  <Package ...>
    ...
    <Extensions>
      ...
      <Extension Category="windows.activatableClass.outOfProcessServer">
        <OutOfProcessServer ServerName="wherewhere.ProcessForUWP.Delegate">
          <Path>【桌面应用项目的路径，如：ProcessForUWP.Demo.Delegate\ProcessForUWP.Demo.Delegate.exe】</Path>
          <Instancing>singleInstance</Instancing>
          <ActivatableClass ActivatableClassId="【自行选择唯一的字符串 ID】" />
        </OutOfProcessServer>
      </Extension>
    </Extensions>
    ...
  </Package>
  ```

  在桌面项目的 `Program.cs` 中添加：
  ```cs
  internal class Program
  {
      private static void Main(string[] args)
      {
          ...
          Factory.StartWinRTServer(new Guid("【自行选择唯一的字符串 ID】"));
      }
      ...
  }
  ```

  在 UWP 项目中使用前添加：
  ```cs
  ProcessProjectionFactory.DefaultType = RPCType.WinRT;
  ProcessProjectionFactory.ActivatableClassID = new Guid("【自行选择唯一的字符串 ID】");
  ```

在 UWP 项目中即可使用 `RemoteProcess` 类：
```cs
IProcessStatic process = ProcessProjectionFactory.ServerManager.ProcessStatic;
RemoteProcessStartInfo info = new("cmd")
{
    CreateNoWindow = true,
    RedirectStandardError = true,
    RedirectStandardInput = true,
    RedirectStandardOutput = true,
    UseShellExecute = false
};
RemoteProcess _process = process.Start(info);
_process.OutputDataReceived += OnOutputDataReceived;
_process.ErrorDataReceived += OnErrorDataReceived;
_process.BeginErrorReadLine();
_process.BeginOutputReadLine();
```

在解决方案配置管理器中，三个项目的平台需保持一致。生成都要勾选。部署只需勾选打包项目，UWP 项目和桌面项目都不需部署。  
解决方案的启动项目应设为打包项目。  

## 注意事项
1. 具体使用方法请查看 Demo
2. 请生成唯一的 UUID，避免与其他 COM 组件冲突
3. 调用 `ProcessProjectionFactory.ServerManager` 前请确保已设置 `ProcessProjectionFactory.CLSID` 或 `ProcessProjectionFactory.ActivatableClassID`
4. OOP/WinRT 不支持沙盒与沙盒外通信，设置 `uap5:RunFullTrust="true"` 将只能与全权限应用通信
5. `ProcessForUWP.UWP` 不支持 Native，在 `.NET Native` 或 `C++` 项目中请使用 [ProcessForUWP.Demo.Projection](./Demo/ProcessForUWP.Demo.Projection)
6. OOP/WinRT 服务器暂时不支持 `.NET Framework`，猜测为在 AppContainer 内会运行在 `.NET Core 5.0` 上

## Star 数量统计
[![Star 数量统计](https://starchart.cc/wherewhere/ProcessForUWP.svg?variant=adaptive)](https://starchart.cc/wherewhere/ProcessForUWP "Star 数量统计")
