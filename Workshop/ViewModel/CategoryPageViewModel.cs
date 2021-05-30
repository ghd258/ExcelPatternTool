﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Workshop.Common;
using Workshop.Control;
using Workshop.Core.DataBase;
using Workshop.Core.Domains;
using Workshop.Core.Entites;
using Workshop.Helper;
using Workshop.Infrastructure.Core;
using Workshop.Infrastructure.Helper;
using Workshop.Infrastructure.Models;
using Workshop.Infrastructure.Services;
using Workshop.Model;
using Workshop.View;
namespace Workshop.ViewModel
{
    public class CategoryPageViewModel : ViewModelBase
    {

        private string filePath;

        private readonly WorkshopDbContext _dbContext;
        public CategoryPageViewModel(WorkshopDbContext dbContext)
        {
            this.SubmitCommand = new RelayCommand(SubmitAction, CanSubmit);
            this.CreateCommand = new RelayCommand(CreateAction);
            this.RemoveCommand = new RelayCommand<Employee>(RemoveAction);
            this.EditCommand = new RelayCommand<Employee>(EditAction);
            this.PropertyChanged += CategoryPageViewModel_PropertyChanged;
            this._dbContext = dbContext;
            InitData();

        }

        private void InitData()
        {
            IList<Employee> data = null;

            var task = InvokeHelper.InvokeOnUi<IList<Employee>>(null, () =>
        {
            var result = this._dbContext.Employee.Where(c => true)
                .Include(c => c.EmployeeAccount)
                .Include(c => c.EmployeeSalay)
                .Include(c => c.EmployeeSocialInsuranceAndFund)
                .Include(c => c.EnterpriseSocialInsuranceAndFund)
                .Include(c => c.EmployeeSocialInsuranceDetail)

                .ToList();
            return result;


        }, (t) =>
             {
                 data = t;
                 try
                 {
                     this.EmployeeInfos = new ObservableCollection<Employee>(data);
                     this.EmployeeInfos.CollectionChanged += CategoryInfos_CollectionChanged;


                 }
                 catch (Exception e)
                 {
                     Console.WriteLine(e);
                     throw;
                 }
             });


        }

        private void CategoryPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.EmployeeInfos))
            {
                SubmitCommand.RaiseCanExecuteChanged();

            }
        }

        private void CategoryInfos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            SubmitCommand.RaiseCanExecuteChanged();
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                var result = 0;
                _dbContext.Employee.Update(e.NewItems[0] as Employee);
                result = _dbContext.SaveChanges();
                if (result == 0)
                {
                    MessageBox.Show("更新失败");

                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var result = 0;
                _dbContext.Employee.Remove(e.OldItems[0] as Employee);
                result = _dbContext.SaveChanges();
                if (result == 0)
                {
                    MessageBox.Show("删除失败");

                }


            }


        }

        private void RemoveAction(Employee obj)
        {
            if (obj == null)
            {
                return;

            }
            RemoveCategory(obj);
        }


        internal void RemoveCategory(Employee CategoryInfo)
        {
            if (EmployeeInfos.Any(c => c.Id == CategoryInfo.Id))
            {
                var current = EmployeeInfos.FirstOrDefault(c => c.Id == CategoryInfo.Id);
                EmployeeInfos.RemoveAt(EmployeeInfos.IndexOf(current));
            }
            else
            {
                MessageBox.Show("条目不存在");

            }
        }

        private void EditAction(Employee obj)
        {
            if (obj == null)
            {
                return;

            }
            var childvm = SimpleIoc.Default.GetInstance<CreateCategoryViewModel>();
            childvm.CurrentEmployee = obj;

            var cpwindow = new CreateCategoryWindow();
            cpwindow.ShowDialog();

        }



        private void CreateAction()
        {
            var cpwindow = new CreateCategoryWindow();
            cpwindow.ShowDialog();
        }

        private void SubmitAction()
        {
            var odInfos = EmployeeInfos.ToList();



            if (odInfos.Count > 0)
            {

                var defaultFontName = AppConfigurtaionService.Configuration["HeaderDefaultStyle:DefaultFontName"];
                var defaultFontColor = AppConfigurtaionService.Configuration["HeaderDefaultStyle:DefaultFontColor"];
                short defaultFontSize = Convert.ToInt16(AppConfigurtaionService.Configuration["HeaderDefaultStyle:DefaultFontSize"]);
                var defaultBorderColor = AppConfigurtaionService.Configuration["HeaderDefaultStyle:DefaultBorderColor"];
                var defaultBackColor = AppConfigurtaionService.Configuration["HeaderDefaultStyle:DefaultBackColor"];

                var employeeEntitys = AutoMapperHelper.MapToList<Employee, EmployeeEntity>(this.EmployeeInfos, new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<object, IStyledType>().ConvertUsing((s, d) =>
                    {
                        IStyledType result = null;
                        switch (s)
                        {
                            case string stringValue:
                                result = new StyledType<string>(stringValue);
                                break;
                            case short _:
                            case long _:
                            case int _:
                                result = new StyledType<long>((long)s);
                                break;
                            case float _:
                            case double _:
                            case decimal _:
                                result = new StyledType<double>(double.Parse(s.ToString()));
                                break;
                            case DateTime dateTimeValue:
                                result = new StyledType<DateTime>(dateTimeValue);
                                break;
                            case bool boolValue:
                                result = new StyledType<bool>(boolValue);
                                break;
                        }
                        return result;
                    });
                    cfg.CreateMap<object, ICommentedType>().ConvertUsing((s, d) =>
                    {
                        ICommentedType result = null;
                        switch (s)
                        {
                            case string stringValue:
                                result = new CommentedType<string>(stringValue);
                                break;
                            case short _:
                            case long _:
                            case int _:
                                result = new CommentedType<long>((long)s);
                                break;
                            case float _:
                            case double _:
                            case decimal _:
                                result = new CommentedType<double>(double.Parse(s.ToString()));
                                break;
                            case DateTime dateTimeValue:
                                result = new CommentedType<DateTime>(dateTimeValue);
                                break;
                            case bool boolValue:
                                result = new CommentedType<bool>(boolValue);
                                break;
                        }
                        return result;
                    });
                    cfg.CreateMap<object, IFormulatedType>().ConvertUsing((s, d) =>
                    {
                        IFormulatedType result = null;
                        switch (s)
                        {
                            case string stringValue:
                                result = new FormulatedType<string>(stringValue);
                                break;
                            case short _:
                            case long _:
                            case int _:
                                result = new FormulatedType<long>((long)s);
                                break;
                            case float _:
                            case double _:
                            case decimal _:
                                result = new FormulatedType<double>(double.Parse(s.ToString()));
                                break;
                            case DateTime dateTimeValue:
                                result = new FormulatedType<DateTime>(dateTimeValue);
                                break;
                            case bool boolValue:
                                result = new FormulatedType<bool>(boolValue);
                                break;
                        }
                        return result;
                    });
                }));
                var aa = employeeEntitys;
                //var task = InvokeHelper.InvokeOnUi<IEnumerable<EmployeeEntity>>(null, () =>
                //{

                //    return employeeEntitys;



                //}, async (t) =>
                //{

                //});
            }

        }


        private ObservableCollection<Employee> _categoryTypeInfos;

        public ObservableCollection<Employee> EmployeeInfos
        {
            get
            {
                if (_categoryTypeInfos == null)
                {
                    _categoryTypeInfos = new ObservableCollection<Employee>();
                }
                return _categoryTypeInfos;
            }
            set
            {
                _categoryTypeInfos = value;
                RaisePropertyChanged(nameof(EmployeeInfos));
            }
        }

        public void CreateCategory(Employee model)
        {
            var id = Guid.NewGuid();
            var createtime = DateTime.Now;
            model.Id = id;
            model.CreateTime = createtime;
            if (EmployeeInfos.Any(c => c.Id == model.Id))
            {
                var current = EmployeeInfos.FirstOrDefault(c => c.Id == model.Id);
                EmployeeInfos[EmployeeInfos.IndexOf(current)] = model;
            }
            else
            {
                EmployeeInfos.Add(model);

            }
        }


        private bool CanSubmit()
        {
            return this.EmployeeInfos.Count > 0;

        }


        public RelayCommand GetDataCommand { get; set; }

        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand CreateCommand { get; set; }
        public RelayCommand<Employee> EditCommand { get; set; }
        public RelayCommand<Employee> RemoveCommand { get; set; }

    }

}
