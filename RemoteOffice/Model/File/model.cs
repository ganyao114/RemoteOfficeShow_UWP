using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace file
{
    public class model
    {
        public model()
        {
            //C# uwp应用的文件读写最常见错误
            //就是没有权限。
            //而最简单的方法是对已知的文件
            //路径进行访问
            //自身的路径
            ce();

        }
        /// <summary>
        /// 结果
        /// </summary>
        public string result
        {
            set
            {
                _result.Add(value);
            }
            get
            {
                StringBuilder t = new StringBuilder();
                foreach (string s in _result)
                {
                    t.Append(s);
                }
                return t.ToString();
            }
        }
        public async void ce()
        {
            //创建文件
            await create_ApplicationData("测试.txt");
            //创建文件夹
            await folder_ApplicationData("在安装的路径创建不需要权限");

            //读写文件和以前一样
            await write_ApplicationData();
            await read_ApplicationData();

            //如果要读写别的地方的文件，可以用
            //folder_demonstration = KnownFolders.PicturesLibrary;
            //要在清单请求权限
            //DownloadsFolder 下载文件夹这里也可以
            //folder_demonstration = DownloadsFolder.CreateFolderAsync(folder_name);
            //也可以让用户选


        }
        private StorageFile file_demonstration;//UWP 采用StorageFile来读写文件
        private StorageFolder folder_demonstration;//folder来读写文件夹
        /// <summary>
        /// 自身路径创建文件
        /// </summary>
        /// <param name="file_name">要创建文件名字</param>
        /// <returns>已经创建的文件</returns>
        public async Task create_ApplicationData(string file_name)
        {
            StorageFolder folder;
            folder = ApplicationData.Current.LocalFolder;
            //Current的值可以是
            //LocalCacheFolder 本地临时文件夹
            //LocalFolder 本地文件夹
            //LocalSettings 本地设置
            //RoamingFolder 漫游文件夹
            //RoamingSettings 漫游设置

            file_demonstration = await folder.CreateFileAsync(file_name , CreationCollisionOption.ReplaceExisting);
            //CreationCollisionOption 可以选择           
            //     如果文件或文件夹已存在，则自动为指定名称的基础追加一个编号。例如，如果 MyFile.txt 已存在，则新文件名为 MyFile (2).txt。如果 MyFolder
            //     已存在，则新文件夹名为 MyFolder (2)。
            //GenerateUniqueName         
            //ReplaceExisting 如果文件或文件夹已存在，则替换现有的项。            
            //FailIfExists 默认值 如果文件或文件夹已存在，则引发类型为 System.Exception 的异常。        
            //OpenIfExists 如果文件或文件夹已存在，则返回现有的项。

            notify("创建文件成功，文件路径" + file_demonstration.Path);
        }

        /// <summary>
        /// 自身路径创建文件夹
        /// </summary>
        /// <param name="file_name">要创建文件夹名字</param>
        /// <returns>已经创建的文件夹</returns>
        public async Task folder_ApplicationData(string folder_name)
        {
            StorageFolder folder;
            folder = ApplicationData.Current.LocalFolder;
            folder_demonstration = await folder.CreateFolderAsync(folder_name , CreationCollisionOption.ReplaceExisting);
            notify("创建文件夹成功，文件夹路径" + folder_demonstration.Path);
        }

        /// <summary>
        /// 对自身路径进行文件读写
        /// </summary>
        public async Task write_ApplicationData()
        {
            using (Stream file = await file_demonstration.OpenStreamForWriteAsync())
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.Write("写入");
                }
            }

            //using (StorageStreamTransaction transaction = await file_demonstration.OpenTransactedWriteAsync())
            //{
            //    using (DataWriter dataWriter = new DataWriter(transaction.Stream))
            //    {
            //        dataWriter.WriteString(str);
            //        transaction.Stream.Size = await dataWriter.StoreAsync();
            //        await transaction.CommitAsync();
            //    }
            //}

            notify("写入文件成功");
        }
        /// <summary>
        /// 读文件
        /// </summary>
        public async Task read_ApplicationData()
        {
            string s;
            using (Stream file = await file_demonstration.OpenStreamForReadAsync())
            {
                using (StreamReader read = new StreamReader(file))
                {
                    s = read.ReadToEnd();
                }
            }
            notify("读文件\"" + s + "\"");
        }
        public async Task<StorageFile> file_open()
        {
            Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            //显示方式
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            //选择最先的位置
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            //后缀名
            picker.FileTypeFilter.Add(".ppt");
            picker.FileTypeFilter.Add(".pptx");
            picker.FileTypeFilter.Add(".doc");
            picker.FileTypeFilter.Add(".docx");
            picker.FileTypeFilter.Add(".xls");
            picker.FileTypeFilter.Add(".xlsx");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {               
                notify("选择 " + file.Name);
                file_demonstration = file;
            }
            return file;
        }
        public async Task folder_open()
        {
            Windows.Storage.Pickers.FolderPicker folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            folderPicker.FileTypeFilter.Add(".txt");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                notify("选择" + folder.Name);
                folder_demonstration = folder;
            }
        }

        private List<string> _result = new List<string>();
        private void notify(string str)
        {
            _result.Add(str + "\r\n");
        }

    }
}
