using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Bouyei.ProviderFactory.DbRemotingProvider
{
    using DbAdoProvider;

    /// <summary>
    /// dbLayer对象的Remoting服务发布和接收类
    /// </summary>
    [Serializable]
    public class DbRemoting : BaseRemoting
    {
        sealed class Singleton
        {
            static Singleton()
            { }

            internal static DbRemoting remotingChannel = new DbRemoting();
        }

        /// <summary>
        /// 构造
        /// </summary>
        public DbRemoting()
            : base(false)
        {
        }

        /// <summary>
        ///参数构造
        /// </summary>
        /// <param name="isSecurity"></param>
        public DbRemoting(bool isSecurity) :
            base(isSecurity)
        {

        }

        /// <summary>
        /// 单例类实例化
        /// </summary>
        public static DbRemoting Instance
        {
            get
            {
                return Singleton.remotingChannel;
            }
        }

        /// <summary>
        /// 获取服务端对象
        /// </summary>
        /// <param name="channelType"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public DbLayerR GetServerChannel(ChannelType channelType, string ip, int port, string channelName)
        {
            return (DbLayerR)Activator.GetObject(typeof(DbLayerR), GetServerChannelUri(channelType, ip, port, channelName));
        }

        /// <summary>
        /// 获取客户端对象
        /// </summary>
        /// <param name="channelType"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public DbLayer GetClientChannel(ChannelType channelType, string ip, int port, string channelName)
        {
            return (DbLayer)Activator.GetObject(typeof(DbLayer), GetClientChannelUri(channelType, ip, port, channelName));
        }

        /// <summary>
        /// 设置客户端对象
        /// </summary>
        /// <param name="dbLayer"></param>
        public void SetClientChannel(DbLayerR dbLayer)
        {
            SetClientChannelObject(dbLayer);
        }

        /// <summary>
        /// 设置服务端对象
        /// </summary>
        /// <param name="dbLayer"></param>
        /// <param name="channelName"></param>
        public void SetServerChannel(DbLayerR dbLayer, string channelName)
        {
            SetServerChannelObject(dbLayer, channelName);
        }

        /// <summary>
        /// 注销通道
        /// </summary>
        /// <param name="dbLayer"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public bool CancellationChannel(DbLayerR dbLayer, string channelName)
        {
            if (dbLayer != null)
            {
                if (DisconnectChannel(dbLayer))
                {
                    ChannelUnregister(channelName);
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
