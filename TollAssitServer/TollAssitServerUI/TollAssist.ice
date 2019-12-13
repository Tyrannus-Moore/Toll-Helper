// ASSIST接口
#ifndef ASSIST_ICE
#define ASSIST_ICE

module ASSISTICE
{
	//车辆信息
	struct CarTable
	{
		string id;//车辆信息id
		string color; //车牌颜色
		string number;//车牌号
		string brand;//车辆品牌
		string type;//车辆类型
		int axleNum;//轴数
		int maxPassenger;// 核定载客
		double maxLoad;// 核定载重量kg
		double weight;// 整备质量kg
		string creator;// 创建人
		string company;// 创建单位
		int monLevel;//监视级别
		string esType;// 逃费类型(来自于es_car_info表)
		string esRemark;// 备注(来自于es_car_info表)
		string dtime; //创建时间
	};
	
	//收费节点信息
	struct TollNode
	{
		string companycode;
		string plazcode;
		string lanname;
		int lannum;
	};
	
	//收费站信息
	struct Station
	{
		string bm;
		string lb;
		string mc;
		string lgs;
	};
	
	//批量查询参数
	struct BatchQueryParams
	{
		string platte;
		TollNode node;
	};
	
	
	
	//消费记录
	struct CustomRecord
	{
		string id;//车辆信息id
		string number;//车牌号
		string color; //车牌颜色
		string brand;//车辆品牌
		string type;//车辆类型
		string flag;//串口传递过来的除车牌号之外的字段信息
		int monLevel;//监视级别
		string dtime;//消费时间
		string operator;//操作员
		double customamount;//消费总计
		TollNode node;
		
	};
	
	//车辆记录
	struct CarRecord
	{
		string number;//车牌号
		string color;//车辆颜色
		int monLevel;//监视级别
		string type;//车辆类型
		string flag;//串口传递过来的除车牌号之外的字段信息
		string companycode;
		string plazcode;
		string lanname;
		int lannum;
		string dtime;
	};
	
	sequence<Station> Stations;//站点集合
	sequence<BatchQueryParams> BatchQuerys;
	sequence<CustomRecord> CustomRecords;
	sequence<CarTable> CarTables;//20171217 add
	
	interface ICarQuery
	{
		//20171217 update
		//查询车辆记录,异步返回结果
		//flag：串口传递过来的除车牌号之外的字段信息
		["ami", "amd"] bool QueryCarRecord(string platte,string flag,TollNode node,out CarTables records,out string error);
		
		//批量查询,不需要返回结果，此接口的用途类似于缓存查询
		bool BatchQuery(BatchQuerys querys,out string error);
		
		//批量上传消费信息
		bool BatchUpload(CustomRecords records,out string error);
		
		//查询站点集合
		bool QueryStations(int from,int count,out Stations lst,out string error);
		
		//20171217 add
		//上传收费节点信息
		//flag:节点唯一标识
		//node:节点信息
		//备注：服务端根据flag查询数据库如果不存在则添加记录存在则更新记录
		bool UploadTollNode(string flag,TollNode node,out string error);
	};

};



#endif