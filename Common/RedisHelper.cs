using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Redis;
using System.Linq;
using ServiceStack.Text;
using System.Text;

namespace Common
{
    public class RedisHelper : IDisposable
    {
        /*copyright@2013 All Rights Reserved
         * servicestack.redis为github中的开源项目
         * redis是一个典型的k/v型数据库
         * redis共支持五种类型的数据 string,list,hash,set,sortedset
         * 
         * string是最简单的字符串类型
         * 
         * list是字符串列表，其内部是用双向链表实现的，因此在获取/设置数据时可以支持正负索引
         * 也可以将其当做堆栈结构使用
         * 
         * hash类型是一种字典结构，也是最接近RDBMS的数据类型，其存储了字段和字段值的映射，但字段值只能是
         * 字符串类型，散列类型适合存储对象，建议使用对象类别和ID构成键名，使用字段表示对象属性，字
         * 段值存储属性值，例如：car:2 price 500 ,car:2  color black,用redis命令设置散列时，命令格式
         * 如下：HSET key field value，即key，字段名，字段值
         * 
         * set是一种集合类型，redis中可以对集合进行交集，并集和互斥运算
         *           
         * sorted set是在集合的基础上为每个元素关联了一个“分数”，我们能够
         * 获得分数最高的前N个元素，获得指定分数范围内的元素，元素是不同的，但是"分数"可以是相同的
         * set是用散列表和跳跃表实现的，获取数据的速度平均为o(log(N))
         * 
         * 需要注意的是，redis所有数据类型都不支持嵌套
         * redis中一般不区分插入和更新操作，只是命令的返回值不同
         * 在插入key时，如果不存在，将会自动创建

         *
         * 以下方法为基本的设置数据和取数据
         */

        ///// <summary>
        ///// 连接池管理器
        ///// </summary>
        ///// <param name="readWriteHosts"></param>
        ///// <param name="readOnlyHosts"></param>
        ///// <returns></returns>
        private static PooledRedisClientManager CreateManager(string[] readWriteHosts, string[] readOnlyHosts)
        {
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts,
                new RedisClientManagerConfig
                {
                    MaxWritePoolSize = readWriteHosts.Length * 2,
                    MaxReadPoolSize = readOnlyHosts.Length * 2,
                    AutoStart = true,
                });
        }

        private static string ReadWriteHosts
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["RedisReadWriteHosts"]; }
        }

        private static string ReadOnlyHosts
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["RedisReadOnlyHosts"]; }
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        private readonly static PooledRedisClientManager PooleClient = CreateManager(new[] { ReadWriteHosts }, new[] { ReadOnlyHosts });

        /// <summary>
        /// 获取key,返回string格式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueString(string key)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                string value = redis.GetValue(key);
                return value;
            }
        }

        /// <summary>
        /// 返回Redis是否包含当前KEY
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(string key)
        {
            try
            {
                using (IRedisClient redis = PooleClient.GetClient())
                {
                    return redis.ContainsKey(key);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 返回Redis hash是否包含当前KEY下的字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsHashFieldKey(string key, string field)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                return redis.HashContainsEntry(key, field);
            }
        }

        /// <summary>
        /// 获取key,返回泛型格式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                return redis.Get<T>(key);
            }
        }
        /// <summary>
        /// 获取Key,返回多个值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValues(string key)
        {
            try
            {
                using (IRedisClient redis = PooleClient.GetClient())
                {
                    return string.Join("#", redis.GetValues(key.Split(',').ToList()));
                }
            }
            catch (Exception ex)
            {
                return "fail";
            }
        }
        /// <summary>
        /// 设置key,传入泛型格式
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void Set<T>(string key, T value)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                if (!redis.Set(key, value))
                {
                    if (redis.ContainsKey(key) && !redis.Remove(key)) throw new Exception(string.Format("redis产生脏数据,{0}=>{1}", key, value));
                }
            }
        }

        /// <summary>
        /// 设置key,传入泛型格式和过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="exp"></param>
        public static void Set<T>(string key, T value, DateTime exp)
        {
            try
            {
                using (IRedisClient redis = PooleClient.GetClient())
                {
                    if (!redis.Set(key, value, exp))
                    {
                        if (redis.ContainsKey(key) && !redis.Remove(key)) throw new Exception(string.Format("redis产生脏数据,{0}=>{1}", key, value));
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void Remove(string key)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                if (redis.ContainsKey(key))
                    if (!redis.Remove(key)) throw new Exception(string.Format("redis移除失败,key={0}", key));
            }
        }

        /// <summary>
        /// 删除hash指定字段值
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static void RemoveHash(string hashId, string field)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                if (redis.HashContainsEntry(hashId, field))
                    if (!redis.RemoveEntryFromHash(hashId, field)) throw new Exception(string.Format("redis移除失败,hashkey={0}", hashId));
            }
        }

        /// <summary>
        /// 事务回滚删除key(异常打印紧急日志，仅供事务回滚调用)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void RollbackRemove(string key)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                if (redis.ContainsKey(key)) redis.Remove(key);
            }
        }

        /// <summary>
        /// 获得某个hash型key下的所有字段
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public static List<string> GetHashFields(string hashId)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                List<string> hashFields = redis.GetHashKeys(hashId);
                return hashFields;
            }
        }

        /// <summary>
        /// 获得某个hash型key下的所有值
        /// </summary>
        /// <param name="hashId"></param>
        /// <returns></returns>
        public static List<string> GetHashValues(string hashId)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                List<string> hashValues = redis.GetHashValues(hashId);
                return hashValues;
            }
        }

        /// <summary>
        /// 根据通配符获取符合条件的key集合
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static List<string> GetKeysList(string pattern)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                List<string> keysList = redis.SearchKeys(pattern);
                return keysList;
            }
        }

        /// <summary>
        /// 获得hash型key某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        public static string GetHashField(string key, string field)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                string value = redis.GetValueFromHash(key, field);
                return value;
            }
        }

        /// <summary>
        /// 设置hash型key某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void SetHashField(string key, string field, string value)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                redis.SetEntryInHash(key, field, value);
            }
        }

        /// <summary>
        ///使某个字段增加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="incre"></param>
        /// <returns></returns>
        public static void SetHashIncr(string key, string field, int incre)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                redis.IncrementValueInHash(key, field, incre);
            }

        }

        /// <summary>
        /// 向list类型数据添加成员，向列表底部(右侧)添加
        /// </summary>
        /// <param name="item"></param>
        /// <param name="list"></param>
        public static void AddItemToListRight(string list, string item)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                redis.AddItemToList(list, item);
            }
        }

        /// <summary>
        /// 从list类型数据读取所有成员
        /// </summary>
        public static List<string> GetAllItems(string list)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                List<string> listMembers = redis.GetAllItemsFromList(list);
                return listMembers;
            }
        }

        /// <summary>
        /// 从list类型数据指定索引处获取数据，支持正索引和负索引
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetItemFromList(string list, int index)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                string item = redis.GetItemFromList(list, index);
                return item;
            }
        }

        public static string PopItemFromList(string listid)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                string item = redis.PopItemFromList(listid);
                return item;
            }
        }

        /// <summary>
        /// 向列表底部（右侧）批量添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="values"></param>
        public static void GetRangeToList(string list, List<string> values)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                redis.AddRangeToList(list, values);
            }
        }

        /// <summary>
        /// 向集合中添加数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="set"></param>
        public static void GetItemToSet(string item, string set)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                redis.AddItemToSet(item, set);
            }
        }

        /// <summary>
        /// 获得集合中所有数据
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static HashSet<string> GetAllItemsFromSet(string set)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                HashSet<string> items = redis.GetAllItemsFromSet(set);
                return items;
            }
        }

        /// <summary>
        /// 获取fromSet集合和其他集合不同的数据
        /// </summary>
        /// <param name="fromSet"></param>
        /// <param name="toSet"></param>
        /// <returns></returns>
        public static HashSet<string> GetSetDiff(string fromSet, params string[] toSet)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                HashSet<string> diff = redis.GetDifferencesFromSet(fromSet, toSet);
                return diff;
            }
        }

        /// <summary>
        /// 获得所有集合的并集
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static HashSet<string> GetAllItemsFromSortedSetDescGetSetUnion(params string[] set)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                HashSet<string> union = redis.GetUnionFromSets(set);
                return union;
            }
        }

        /// <summary>
        /// 获得所有集合的交集
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static HashSet<string> GetSetInter(params string[] set)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                HashSet<string> inter = redis.GetIntersectFromSets(set);
                return inter;
            }
        }


        /// <summary>
        /// 对集合中指定元素的权重+1（线程安全）
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        public static void IncrementItemInSortedSet(string set, string value)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                redis.IncrementItemInSortedSet(set, value, 1);
            }
        }

        /// <summary>
        /// 在有序集合中删除元素
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        public static void RemoveItemFromSortedSet(string set, string value)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                if (redis.SortedSetContainsItem(set, value)) redis.RemoveItemFromSortedSet(set, value);
            }
        }

        public static List<string> GetAllItemsFromSortedSet(string set)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                return redis.GetAllItemsFromSortedSet(set);
            }
        }

        /// <summary>
        /// 降序查询有序集合中包含指定关键字的序列
        /// </summary>
        /// <param name="set"></param>
        /// <param name="key"></param>
        /// <param name="take"></param>
        public static List<string> GetAllItemsFromSortedSetDescOld(string set, string key, int take)
        {
            key = key.ToLower();
            using (IRedisClient redis = PooleClient.GetClient())
            {
                return redis.GetAllItemsFromSortedSetDesc(set).Where(c => c.ToLower().Contains(key)).Take(take).ToList();
            }
        }

        /// <summary>
        /// 获得某个值在有序集合中的排名，按分数的降序排列
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetItemIndexInSortedSetDesc(string set, string value)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                long index = redis.GetItemIndexInSortedSetDesc(set, value);
                return (int)index;
            }
        }

        /// <summary>
        /// 获得某个值在有序集合中的排名，按分数的升序排列
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetItemIndexInSortedSet(string set, string value)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                long index = redis.GetItemIndexInSortedSet(set, value);
                return (int)index;
            }
        }

        /// <summary>
        /// 获得有序集合中某个值得分数
        /// </summary>
        /// <param name="set"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetItemScoreInSortedSet(string set, string value)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                double score = redis.GetItemScoreInSortedSet(set, value);
                return score;
            }
        }

        /// <summary>
        /// 获得有序集合中，某个排名范围的所有值
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginRank"></param>
        /// <param name="endRank"></param>
        /// <returns></returns>
        public static List<string> GetRangeFromSortedSet(string set, int beginRank, int endRank)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                List<string> valueList = redis.GetRangeFromSortedSet(set, beginRank, endRank);
                return valueList;
            }
        }

        /// <summary>
        /// 获得有序集合中，某个分数范围内的所有值，升序
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginScore"></param>
        /// <param name="endScore"></param>
        /// <returns></returns>
        public static List<string> GetRangeFromSortedSet(string set, double beginScore, double endScore)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                List<string> valueList = redis.GetRangeFromSortedSetByHighestScore(set, beginScore, endScore);
                return valueList;
            }
        }

        /// <summary>
        /// 获得有序集合中，某个分数范围内的所有值，降序
        /// </summary>
        /// <param name="set"></param>
        /// <param name="beginScore"></param>
        /// <param name="endScore"></param>
        /// <returns></returns>
        public static List<string> GetRangeFromSortedSetDesc(string set, double beginScore, double endScore)
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                List<string> vlaueList = redis.GetRangeFromSortedSetByLowestScore(set, beginScore, endScore);
                return vlaueList;
            }
        }

        public void Dispose()
        {
            using (IRedisClient redis = PooleClient.GetClient())
            {
                redis.Dispose();
            }
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        public static void Subscription(string tochannel, Action<string, string> action)
        {
            using (IRedisClient consumer = PooleClient.GetClient())
            {
                //创建订阅
                ServiceStack.Redis.IRedisSubscription subscription = consumer.CreateSubscription();

                //接收消息处理Action
                subscription.OnMessage = (channel, msg) =>
                {
                    if (action != null) action(channel, msg);
                };

                //订阅频道
                subscription.SubscribeToChannels(tochannel);
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message"></param>
        public static void PublishMessage(string tochannel, string message)
        {
            using (IRedisClient publisher = PooleClient.GetClient())
            {
                publisher.PublishMessage(tochannel, message);
            }
        }

        /// <summary>
        /// list先进先出(右进左出)
        /// </summary>
        /// <param name="listid"></param>
        /// <returns></returns>
        public static List<string> GetListLPop(string listid)
        {
            using (var native = PooleClient.GetClient())
            {
                var client = (RedisNativeClient)native;
                var list = new List<string>();
                long len = client.LLen(listid);
                for (int i = 0; i < len; i++)
                {
                    var temp = client.LPop(listid).FromUtf8Bytes();
                    if (!string.IsNullOrWhiteSpace(temp)) list.Add(temp);
                }
                return list;
            }
        }

        /// <summary>
        /// 右出
        /// </summary>
        /// <param name="listid"></param>
        /// <returns></returns>
        public static List<string> GetListRightPop(string listid)
        {
            using (var client = PooleClient.GetClient())
            {
                var list = new List<string>();
                long len = client.GetListCount(listid);
                for (int i = 0; i < len; i++)
                {
                    var temp = client.PopItemFromList(listid);
                    if (!string.IsNullOrWhiteSpace(temp)) list.Add(temp);
                }
                return list;
            }
        }

        /// <summary>
        /// list先进先出(左进右出)
        /// </summary>
        /// <param name="listid"></param>
        /// <returns></returns>
        public static List<string> GetListRPop(string listid)
        {
            using (var native = PooleClient.GetClient())
            {
                var client = (RedisNativeClient)native;
                var list = new List<string>();
                long len = client.LLen(listid);
                for (int i = 0; i < len; i++)
                {
                    var temp = client.RPop(listid).FromUtf8Bytes();
                    if (!string.IsNullOrWhiteSpace(temp)) list.Add(temp);
                }
                return list;
            }
        }

        /// <summary>
        /// 消息队列消息入队
        /// </summary>
        public static void EnqueueItemOnList(string listid, string value)
        {
            try
            {
                using (IRedisClient client = PooleClient.GetClient())
                {
                    //将信息入队
                    client.EnqueueItemOnList(listid, value);
                }
            }
            catch (Exception ex)
            {
                return;
            }

        }

        /// <summary>
        /// 消息队列消息入队
        /// </summary>
        //public static void EnqueueItemOnListAndSendMessage(string listid, string value)
        //{
        //    using (IRedisClient client = PooleClient.GetClient())
        //    {
        //        //将信息入队
        //        client.EnqueueItemOnList(listid, value);
        //        client.PublishMessage(RedisShareDataConst.alarm_recard_push_channel, "message");
        //    }
        //}
    }
}
