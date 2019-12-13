// ASSIST接口
#ifndef ASSIST_UPDATE_MODULE_ICE
#define ASSIST_UPDATE_MODULE_ICE

module ASSISTUPDATEMODULEICE
{
	//资源项
	struct ResourceItem
	{
		string resourceFolderName;//发布资源文件夹名称yyyyMMddHHmmss
		string resourcePath;//详细资源路径,这里并不包含上级资源文件夹(Resource)路径
	};
	//更新类型
	enum UpdateType
	{
		Other,//其他
		Client,//客户端
		Server,//服务端
	};
	
	sequence<byte> Resource;//资源
	sequence<ResourceItem> ResourceList;
	
	//更新接口
	interface IUpdate
	{
		
		//检查更新
		//localSerialNumber：本地版本号
		//type:选择要检查更新的软件
		//list：返回需要更新的资源列表
		//newSerialNumber:最新的版本号
		//error：错误消息
		//返回值：成功返回true，成功返回后需要检查list
		["amd"] bool CheckUpdate(long localSerialNumber,UpdateType type,out ResourceList list,out long newSerialNumber,out string error);
		
		//查询资源
		//type：资源所属
		//item：需要查询的资源
		//queryId：返回资源Id
		//error：错误消息
		//返回值：成功返回true
		["amd"] bool QueryResource(UpdateType type,ResourceItem item,out long queryId,out string error);
		
		//获取资源
		//queryId：调用QueryResource获取的queryId
		//from：资源偏移位置
		//count:需要取的资源大小
		//data：返回获取到的资源
		//error：错误消息
		//返回值：成功返回true，如果返回true&&data的长度为0则资源已经取完
		["amd"] bool GetResource(long queryId,int from,int count, out Resource data,out string error);
		
		//关闭资源，以释放服务端资源
		//queryId：调用QueryResource获取的queryId
		//error：错误消息
		//返回值：成功返回true
		["amd"] bool CloseResource(long queryId,out string error);
		
	};

};



#endif