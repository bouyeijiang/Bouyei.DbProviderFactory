using System;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;

namespace Bouyei.ProviderFactory.DbRemotingProvider
{
    /// <summary>
    /// 远程执行对象
    /// </summary>
    [Serializable]
    public class BaseRemoting
    {
        TcpServerChannel tcpServerChannel = null;
        HttpServerChannel httpServerChannel = null;
        IpcServerChannel ipcServerChannel = null;

        TcpClientChannel tcpClientChannel = null;
        HttpClientChannel httpClientChannel = null;
        IpcClientChannel ipcClientChannel = null;
        ObjRef objRef = null;
        bool isSecurity = false;

        /// <summary>
        /// 构造
        /// </summary>
        public BaseRemoting()
        {
            this.isSecurity = false;
            try
            {
                if (RemotingConfiguration.CustomErrorsMode != CustomErrorsModes.Off)
                {
                    RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
                    RemotingConfiguration.CustomErrorsEnabled(false);
                }
            }
            finally
            { }
        }

        /// <summary>
        ///参数构造
        /// </summary>
        /// <param name="isSecurity"></param>
        public BaseRemoting(bool isSecurity=false)
        {
            this.isSecurity = isSecurity;
            try
            {
                if (RemotingConfiguration.CustomErrorsMode != CustomErrorsModes.Off)
                {
                    RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
                    RemotingConfiguration.CustomErrorsEnabled(false);
                }
            }
            finally
            { }
        }

        /// <summary>
        /// 设置远程服务模式,为单对象服务或者为多对象服务
        /// </summary>
        /// <param name="type">远程对象类型</param>
        /// <param name="channelName">通道名称</param>
        /// <param name="isSingleton">是否为单对象服务,默认为true,如果为false则当有remoting请求则新生成一个对象</param>
        public void SetRemotingMode(Type type,string channelName,bool isSingleton = true)
        {
            RemotingConfiguration.RegisterWellKnownServiceType(type,
                channelName,
               (isSingleton ? WellKnownObjectMode.Singleton : WellKnownObjectMode.SingleCall));
        }

        #region 服务端操作
        /// <summary>
        /// 注册服务通道
        /// </summary>
        /// <param name="channelType">通道类型</param>
        /// <param name="channelName">资源名称</param>
        /// <param name="port">端口</param>
        public void RegisterServerChannel(ChannelType channelType, string channelName, int port)
        {
            try
            {
                switch (channelType)
                {
                    case ChannelType.Tcp:
                        {
                            if (tcpServerChannel != null)
                            {
                                if (tcpServerChannel.ChannelName == channelName)
                                    return;
                            }
                            tcpServerChannel = new TcpServerChannel(channelName, port);
                            ChannelServices.RegisterChannel(tcpServerChannel, isSecurity);
                        }
                        break;
                    case ChannelType.Http:
                        {
                            if (httpServerChannel != null)
                            {
                                if (httpServerChannel.ChannelName == channelName)
                                    return;
                            }
                            httpServerChannel = new HttpServerChannel(channelName, port);
                            ChannelServices.RegisterChannel(httpServerChannel, isSecurity);
                        }
                        break;
                    case ChannelType.Ipc:
                        {
                            if (ipcServerChannel != null)
                            {
                                if (ipcServerChannel.ChannelName == channelName)
                                    return;
                            }
                            ipcServerChannel = new IpcServerChannel(channelName, port.ToString());
                            ChannelServices.RegisterChannel(ipcServerChannel, isSecurity);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception f)
            {
                throw new Exception(f.Message + "RegisterChannelError");
            }
        }

       /// <summary>
        /// 获得服务通道数据对象,读取服务端设置的对象
       /// </summary>
       /// <param name="channelType">通道类型</param>
       /// <param name="ip">服务地址</param>
       /// <param name="port">服务端口</param>
       /// <param name="channelName">通道名称</param>
       /// <returns></returns>
        public string GetServerChannelUri(ChannelType channelType, string ip, int port, string channelName)
        {
            string url = string.Empty;
            try
            {
                switch (channelType)
                {
                    case ChannelType.Tcp:
                        {
                            url = string.Format("tcp://{0}:{1}/{2}", ip, port, channelName);
                        }
                        break;
                    case ChannelType.Http:
                        {
                            url = string.Format("http://{0}:{1}/{2}", ip, port, channelName);
                        }
                        break;
                    case ChannelType.Ipc:
                        {
                            url = string.Format("ipc://{0}:{1}/{2}", ip, port, channelName);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception f)
            {
                throw new Exception(f.Message + "GetRemotingObjectError");
            }

            return url;
        }

        /// <summary>
        /// 获取远程通道注册的数据对象
        /// </summary>
        /// <param name="type">数据对象类型:typeof(Class)</param>
        /// <param name="uri">资源通道地址</param>
        /// <returns></returns>
        public object GetRemoteObject(Type type, string uri)
        {
           return Activator.GetObject(type, uri);
        }

        /// <summary>
        /// 设置服务通道对象,提供给客户调用的服务端对象
        /// </summary>
        /// <param name="obj">支持远程执行的对象</param>
        /// <param name="channelName">注册资源通道名称/param>
        public void SetServerChannelObject(MarshalByRefObject obj, string channelName)
        {
            try
            {
                objRef = RemotingServices.Marshal(obj, channelName);
            }
            catch (Exception f)
            {
                throw new Exception(f.Message + "SetServerObjectError");
            }
        }

        /// <summary>
        /// 注销对应的通道对象
        /// </summary>
        /// <param name="obj">远程对象</param>
        /// <returns></returns>
        public bool DisconnectChannel(MarshalByRefObject obj)
        {
            try
            {
                if (objRef != null)
                    return RemotingServices.Disconnect(obj);
                return false;
            }
            catch (Exception f)
            {
                throw new Exception(f.Message + "DisconnectChannelError");
            }
        }

        #endregion

        #region 客户端

        /// <summary>
        /// 客户端设置通道对象
        /// </summary>
        /// <param name="mscommandServer"></param>
        public void SetClientChannelObject(MarshalByRefObject obj)
        {
            try
            {
                objRef = RemotingServices.Marshal(obj);
            }
            catch (Exception f)
            { throw new Exception(f.Message + "SetClientObjectError"); }
        }

        /// <summary>
        ///客户端获得通道传送对象
        /// </summary>
        /// <param name="channelType"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public string GetClientChannelUri(ChannelType channelType, string ip, int port, string channelName)
        {
            string url = string.Empty;
            try
            {
                switch (channelType)
                {
                    case ChannelType.Tcp:
                        {
                            url = string.Format("tcp://{0}:{1}/{2}", ip, port, channelName);
                        }
                        break;
                    case ChannelType.Http:
                        {
                            url = string.Format("http://{0}:{1}/{2}", ip, port, channelName);
                        }
                        break;
                    case ChannelType.Ipc:
                        {
                            url = string.Format("ipc://{0}:{1}/{2}", ip, port, channelName);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception f)
            {
                throw new Exception(f.Message + "GetClientObjectError");
            }
            return url;
        }

        /// <summary>
        /// 客户端通道注册
        /// </summary>
        /// <param name="channelType"></param>
        /// <param name="channelName"></param>
        public void RegisterClientChannel(ChannelType channelType, string channelName)
        {
            try
            {
                switch (channelType)
                {
                    case ChannelType.Tcp:
                        {
                            if (tcpClientChannel != null)
                            {
                                if (tcpClientChannel.ChannelName == channelName)
                                    return;
                            }
                            else
                                tcpClientChannel = new TcpClientChannel();

                            ChannelServices.RegisterChannel(tcpClientChannel, isSecurity);
                        }
                        break;
                    case ChannelType.Http:
                        {
                            if (httpClientChannel != null)
                            {
                                if (httpClientChannel.ChannelName == channelName)
                                    return;
                            }
                            else
                                httpClientChannel = new HttpClientChannel();

                            ChannelServices.RegisterChannel(httpClientChannel, isSecurity);
                        }
                        break;
                    case ChannelType.Ipc:
                        {
                            if (ipcClientChannel != null)
                            {
                                if (ipcClientChannel.ChannelName == channelName)
                                    return;
                            }
                            else
                                ipcClientChannel = new IpcClientChannel();

                            ChannelServices.RegisterChannel(ipcClientChannel, isSecurity);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception f)
            {
                throw new Exception(f.Message + "RegisterChannelError");
            }
        }

        /// <summary>
        /// 注销通道
        /// </summary>
        /// <param name="channelName">通道名</param>
        /// <returns></returns>
        public bool ChannelUnregister(string channelName)
        {
            IChannel[] channels = ChannelServices.RegisteredChannels;
            foreach (IChannel ic in channels)
            {
                if (ic.ChannelName == channelName)
                {
                    ChannelServices.UnregisterChannel(ic);
                    return true;
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 生成唯一标识码
        /// </summary>
        /// <returns></returns>
        public string GenerateGuid()
        {
            string guid = System.Guid.NewGuid().ToString();
            char[] newGuid = new char[guid.Length];
            for (int i = guid.Length - 1; i >= 0; i--)
            {
                newGuid[i] = guid[i];
            }
            return new string(newGuid);
        }
    }
}
