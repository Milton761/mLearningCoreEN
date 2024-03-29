﻿using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Core.DownloadCache;
using Core.Session;
using MLearning.Core.Classes;
using MLearning.Core.Configuration;
using MLearning.Core.Entities;
using MLearning.Core.Entities.json;
using MLearning.Core.Services;
using MLearningDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Core.ViewModels
{
    public class LOViewModel : MvxViewModel
    {


        private IMLearningService _mLearningService;


        static int pagecount = 0;


        public LOViewModel(IMLearningService mlearningService)
        {
            _mLearningService = mlearningService;

           
        }




        string _serialized_list;

        public void Init(int lo_id,string serialized_los_in_circle)
        {
            

            //LoadPages(selectedLOIndex);

            LOID = lo_id;

            _serialized_list = serialized_los_in_circle;




        }


       async public Task InitLoad()
        {



            List<lo_by_circle> los_in_Circle = JsonConvert.DeserializeObject<List<lo_by_circle>>(_serialized_list);

            LOsInCircle = new ObservableCollection<lo_by_circle_wrapper>();


            int i = 0;
            foreach (var item in los_in_Circle)
            {

                LOsInCircle.Add(new lo_by_circle_wrapper { lo = item,stack =new stack_wrapper { IsLoaded = false }  });
                

                //Load images if its the selected LO
                bool images = item.id == LOID;

                await LoadPages(i++, images);
            }



            var selectedLOIndex = LOsInCircle.IndexOf(LOsInCircle.Where(lo => lo.lo.id == LOID).First());

             await LoadBackgroundImages();
        }

     
        async Task LoadPages(int loListIndex,bool images)
        {
            var lo_obj = LOsInCircle[loListIndex];
            var LOID = lo_obj.lo.id;

            var list = await _mLearningService.GetPagesByLO(LOID);

            var AllPagesList = new ObservableCollection<page_wrapper>();

            //Loading json content to memory
            foreach (var item in list)
            {
                Mvx.Trace("Id: " + item.id);
                LOContent locontent = null;
                try
                {
                   locontent = JsonConvert.DeserializeObject<LOContent>(item.content);
                    
                }
                catch(Exception e)
                {
                    Mvx.Trace("Serialization error: "+e.Message);
                }
                
                
                AllPagesList.Add(new page_wrapper { page = item, content = locontent /*, id=pagecount*/});
                pagecount++;
            }

         

            

            var tags = await _mLearningService.GetTagsByLO(LOID);

            PageTags = new ObservableCollection<tag_by_page>(tags);

            var tlist = tags.GroupBy(t => t.name).Select(t=>t).ToList();

            var GroupedPagesList = new ObservableCollection<page_collection_wrapper>();

            for (int i = 0; i < tlist.Count;i++ )
            {
                IEnumerable<tag_by_page> group = tlist[i];
                var groupCollection = new ObservableCollection<page_wrapper>();

                foreach (var item in group)
                {
                    groupCollection.Add(AllPagesList.Where(p => p.page.id == item.page_id).First());
                }

                GroupedPagesList.Add(new page_collection_wrapper { PagesList = groupCollection, TagName = group.FirstOrDefault().name });

                if (images)
                    UpdatePagesImages(0, GroupedPagesList[i].PagesList);

                //  GroupedPagesList.Add(AllPagesList.Where(p => p.page.id == item.page_id).First());
            }

            
            LOsInCircle[loListIndex].stack.StacksList = GroupedPagesList;
            LOsInCircle[loListIndex].stack.IsLoaded = true;

            LOCurrentIndex = loListIndex;

            UpdateExtraInfo(LOCurrentIndex); 

            //Download xmls

            //foreach (var item in list)
            //{

            //    await _mLearningService.OpenLOPage(item.url_xml);
            //}


        }

        async Task LoadPageImages(int loindex)
        {
            foreach (var item in LOsInCircle[loindex].stack.StacksList)
	        {                
                UpdatePagesImages(0, item.PagesList);   
            }
        }


        public class page_wrapper : MvxNotifyPropertyChanged
        {
            public Page page { get; set; }

            byte[] _cover_bytes;
            public byte[] cover_bytes
            {
                get { return _cover_bytes; }
                set { _cover_bytes = value; RaisePropertyChanged("cover_bytes"); }
            }

            //public int id { get; set; }
            //public int i_id { get; set; }
            //public int j_id { get; set; }
            //public int k_id { get; set; }



            public LOContent content { get; set; }


            bool _isLoaded;
            public bool IsLoaded
            {
                get { return _isLoaded; }
                set { _isLoaded = value; RaisePropertyChanged("IsLoaded"); }
            }





        }

        public class page_collection_wrapper : MvxNotifyPropertyChanged
        {

            ObservableCollection<page_wrapper> _pagesList;
            public ObservableCollection<page_wrapper> PagesList
            {
                get { return _pagesList; }
                set { _pagesList = value; RaisePropertyChanged("PagesList"); }
            }


            string _tagName;
            public string TagName
            {
                get { return _tagName; }
                set { _tagName = value; RaisePropertyChanged("TagName"); }
            }

        }

        public class stack_wrapper : MvxNotifyPropertyChanged
        {

            ObservableCollection<page_collection_wrapper> _stacksList;
            public ObservableCollection<page_collection_wrapper> StacksList
            {
                get { return _stacksList; }
                set { _stacksList = value; RaisePropertyChanged("StacksList"); }
            }


            bool _isLoaded;
            public bool IsLoaded
            {
                get { return _isLoaded; }
                set { _isLoaded = value; RaisePropertyChanged("IsLoaded"); }
            }




        }

        public class lo_by_circle_wrapper : MvxNotifyPropertyChanged
        {
            public lo_by_circle lo { get; set; }

           

            byte[] _background_bytes;
            public byte[] background_bytes
            {
                get { return _background_bytes; }
                set { _background_bytes = value; RaisePropertyChanged("background_bytes"); }
            }




            stack_wrapper _stack;
            public stack_wrapper stack
            {
                get { return _stack; }
                set { _stack = value; RaisePropertyChanged("stack"); }
            }

            
            stack_wrapper _stack2;
            public stack_wrapper stack2
            {
                get { return _stack; }
                set { _stack2 = value; RaisePropertyChanged("stack2"); }
            }





        }


        int _lOID;
        public int LOID
        {
            get { return _lOID; }
            set { _lOID = value; RaisePropertyChanged("LOID"); }
        }



        page_wrapper _currentPage;
        public page_wrapper CurrentPage
        {
            get { return _currentPage; }
            set {
                _currentPage = value;
                RaisePropertyChanged("CurrentPage");
            }
        }


     

        int _loCurrentIndex;
        public int LOCurrentIndex
        {
            get { return _loCurrentIndex; }
            set { _loCurrentIndex = value; RaisePropertyChanged("LOCurrentIndex"); /*Testing Purposes*/}
        }

       async private Task UpdateExtraInfo(int _loCurrentIndex)
        {
            Title = LOsInCircle[_loCurrentIndex].lo.title;
            Description = LOsInCircle[_loCurrentIndex].lo.description;
            if (LOsInCircle[_loCurrentIndex].stack.StacksList[0].PagesList[0].cover_bytes==null )
            {
                LoadPageImages(_loCurrentIndex);
                
            }
            GroupedPagesList = LOsInCircle[_loCurrentIndex].stack.StacksList;
        }


        string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged("Title"); }
        }


        string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
        }



        ObservableCollection<page_collection_wrapper> _groupedPagesList;
        public ObservableCollection<page_collection_wrapper> GroupedPagesList
        {
            get { return _groupedPagesList; }
            set { _groupedPagesList = value; RaisePropertyChanged("GroupedPagesList"); }
        }



        //Testing Purposes

        ObservableCollection<page_wrapper> _pagesList;
        public ObservableCollection<page_wrapper> PagesList
        {
            get { return _pagesList; }
            set { _pagesList = value; RaisePropertyChanged("PagesList"); }
        }




     




        ObservableCollection<lo_by_circle_wrapper> _losInCircle;
        public ObservableCollection<lo_by_circle_wrapper> LOsInCircle
        {
            get { return _losInCircle; }
            set { _losInCircle = value; RaisePropertyChanged("LOsInCircle"); }
        }



        ObservableCollection<tag_by_page> _pageTags;
        public ObservableCollection<tag_by_page> PageTags
        {
            get { return _pageTags; }
            set { _pageTags = value; RaisePropertyChanged("PageTags"); }
        }






        //ObservableCollection<page_wrapper> _allPagesList;
        //public ObservableCollection<page_wrapper> AllPagesList
        //{
        //    get { return _allPagesList; }
        //    set { _allPagesList = value; RaisePropertyChanged("AllPagesList"); }
        //}

       


        //List of GroupedPagesList


        
        

      

        int _currentIndexDisplaying;
        public int CurrentIndexDisplaying
        {
            get { return _currentIndexDisplaying; }
            set { _currentIndexDisplaying = value; RaisePropertyChanged("CurrentIndexDisplaying"); }
        }



        //Testing purposes


        MvxCommand<page_collection_wrapper> _selectStack;
        public System.Windows.Input.ICommand SelectStack
        {
            get
            {
                _selectStack = _selectStack ?? new MvxCommand<page_collection_wrapper>(DoSelectStack);
                return _selectStack;
            }
        }

        void DoSelectStack(page_collection_wrapper collection)
        {
            PagesList = collection.PagesList;
        }



        MvxCommand<int> _loadStackImagesCommand;
        public System.Windows.Input.ICommand LoadStackImagesCommand
        {
            get
            {
                _loadStackImagesCommand = _loadStackImagesCommand ?? new MvxCommand<int>(DoLoadStackImagesCommand);
                return _loadStackImagesCommand;
            }
        }

        void DoLoadStackImagesCommand(int loindex)
        {
            LoadPageImages(loindex);
        }




        MvxCommand _backCommand;
        public System.Windows.Input.ICommand BackCommand
        {
            get
            {
                _backCommand = _backCommand ?? new MvxCommand(DoBackCommand);
                return _backCommand;
            }
        }

        void DoBackCommand()
        {
            Close(this);
        }


        MvxCommand _homeCommand;
        public System.Windows.Input.ICommand HomeCommand
        {
            get
            {
                _homeCommand = _homeCommand ?? new MvxCommand(DoHomeCommand);
                return _homeCommand;
            }
        }

        void DoHomeCommand()
        {

        }



        MvxCommand _searchCommand;
        public System.Windows.Input.ICommand SearchCommand
        {
            get
            {
                _searchCommand = _searchCommand ?? new MvxCommand(DoSearchCommand);
                return _searchCommand;
            }
        }

        void DoSearchCommand()
        {

        }




        MvxCommand _shareCommand;
        public System.Windows.Input.ICommand ShareCommand
        {
            get
            {
                _shareCommand = _shareCommand ?? new MvxCommand(DoShareCommand);
                return _shareCommand;
            }
        }

        void DoShareCommand()
        {

        }




        MvxCommand _helpCommand;
        public System.Windows.Input.ICommand HelpCommand
        {
            get
            {
                _helpCommand = _helpCommand ?? new MvxCommand(DoHelpCommand);
                return _helpCommand;
            }
        }

        void DoHelpCommand()
        {

        }


       

        MvxCommand<page_wrapper> _openPageCommand;
        public System.Windows.Input.ICommand OpenPageCommand
        {
            get
            {
                _openPageCommand = _openPageCommand ?? new MvxCommand<page_wrapper>(DoOpenPageCommand);
                return _openPageCommand;
            }
        }

        async void DoOpenPageCommand(page_wrapper page)
        { 

            if (!page.IsLoaded)
            {
                await PageParser.DownloadSetBytes(page.content); 
                page.IsLoaded = true;
            }

            //CurrentPage = page;
            addpage2cache(page);
        }


        int MaxPagesCache = 5;
        List<page_wrapper> pagelistcache = new List<page_wrapper>();

        void addpage2cache(page_wrapper page)
        {
            if (pagelistcache.Count >= MaxPagesCache)
            {
                pagelistcache[0].IsLoaded = false;
                PageParser.NullBytes(pagelistcache[0].content);
                pagelistcache.RemoveAt(0);
            }

            pagelistcache.Add(page);

        }



        async Task UpdatePagesImages(int index,ObservableCollection<page_wrapper> list)
        {


            IncrementalDownload _manager = new IncrementalDownload(); ;

            _manager.TryLoadByteVector<page_wrapper>(index, list.ToList()
                , (pos, bytes) =>
                {
                    list[pos].cover_bytes = bytes;
                }
                , (page) =>
                {
                    return page.page.url_img;
                });


        }

        //Not incremental
        async Task LoadBackgroundImages()
        {

           //await BlockDownload.TryLoadByteVector<lo_by_circle_wrapper>(LOsInCircle.ToList(),
           //     (pos, bytes) => { LOsInCircle[pos].background_bytes = bytes; },
           //     (lo) => {return lo.lo.url_background;}
           //     );

          await BlockDownload.TryPutBytesInVector<lo_by_circle_wrapper>(LOsInCircle.ToList(),
                (pos, bytes) => { LOsInCircle[pos].background_bytes = bytes; },
                (lo) => {return lo.lo.url_background;}
               );
        
        }



        async public Task<byte[]> GetBytes(string url)
        {

            CacheService cache = CacheService.Init(SessionService.GetCredentialFileName(), Constants.PreferencesFileName, Constants.LocalDbName);

            var bytesAndLocalPath = await cache.tryGetResource(url);

            return bytesAndLocalPath.Item1;

        }




    }
}
