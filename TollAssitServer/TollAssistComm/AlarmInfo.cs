using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TollAssistComm
{
    /// <summary>
    /// 告警信息类
    /// 备注：告警信息实体
    /// </summary>
    [DataContract(Name = "AlarmInfo")]
    public class AlarmInfo
    {
        [DataMember(Name = "id")]
        public Int64 Id { get; set; }
        [DataMember(Name = "color")]
        public string Color { get; set; }
        [DataMember(Name = "content")]
        public string Content { get; set; }
        [DataMember(Name = "createOrg")]
        public string CreateOrg { get; set; }
        [DataMember(Name = "createTime")]
        public string CreateTime { get; set; }
        [DataMember(Name = "creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 告警信息滚动的结束时间值
        /// 备注:当前时间必须属于[StartTime,EndTime]范围内才进行滚动显示
        /// </summary>
        [DataMember(Name = "endTime")]
        public string EndTime { get; set; }
        [DataMember(Name = "number")]
        public string Number { get; set; }
        [DataMember(Name = "pubOrg")]
        public string PubOrg { get; set; }
        [DataMember(Name = "revocation")]
        public string RevoCation { get; set; }
        [DataMember(Name = "revokeOrg")]
        public string RevokeOrg { get; set; }
        [DataMember(Name = "revokeTime")]
        public string RevokeTime { get; set; }
        /// <summary>
        /// 告警信息滚动的开始时间值
        /// 备注:当前时间必须属于[StartTime,EndTime]范围内才进行滚动显示
        /// </summary>
        [DataMember(Name = "startTime")]
        public string StartTime { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <summary>
        /// 是否已被撤销，0：未撤销；非0：已撤销
        /// 备注：此字段不属于告警信息实体中的字段，本程序使用添加字段
        /// </summary>
        [DataMember(Name = "revoke")]
        public int Revoke { get; set; }

        public override string ToString()
        {
            string formatInfo = string.Format("车牌：{0} 车牌颜色：{1} 消息：{2}",this.Number,this.Color,this.Content);
            return formatInfo;
        }
    }
}
