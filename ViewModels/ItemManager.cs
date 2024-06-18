using Kanbanify.Models;
using Kanbanify.Services;
using Kanbanify.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.ViewModels
{

    public interface IItemManager
    {
        TaskManager TaskManager { get; set; }

        List<TaskModel> Tasks { get; set; }

        Action<ItemModel> ItemAdded { get; set; }

        Action StateHasChanged { get; set; }

        bool HasCompletedTasks(ItemModel item);

        void Archive(ItemModel item);

        void Edit(ItemModel item);

        void Add();
    }

    public class ItemManager : IItemManager
    {

        // =================
        // ===== Properties
        // =================

        public TaskManager TaskManager { get; set; }

        public List<TaskModel> Tasks { get; set; }

        public ItemModel ActiveItem { get; set; }

        // ==============
        // ===== Actions
        // ==============

        public Action<ItemModel> ItemAdded { get; set; }

        public Action StateHasChanged { get; set; }

        // =============
        // ===== Fields
        // =============

        private readonly DatabaseOperations db;

        private readonly ModalService ms;

        // ===================
        // ===== Constructors
        // ===================

        public ItemManager(DatabaseOperations db, ModalService ms)
        {
            this.db = db;
            this.ms = ms;

            TaskManager = new TaskManager(db);
            TaskManager.StateHasChanged += Update;
            TaskManager.TaskAdded += TaskAdded;
        }

        // ========================
        // ===== Interface methods
        // ========================


        public bool HasCompletedTasks(ItemModel item)
        {
            return CheckForCompletedTasks(item);
        }

        public void Edit(ItemModel item)
        {
            ShowModal(item);
        }

        public void Archive(ItemModel item)
        {
            ArchiveItem(item);
            Update();
        }

        public void Add()
        {
            ShowModal();
        }

        // ======================
        // ===== Private methods
        // ======================

        private void Update() =>
            StateHasChanged?.Invoke();

        #region Completed tasks

        private bool CheckForCompletedTasks(ItemModel item)
        {
            var tasks = Tasks
                .Where(x => item.TaskIds.Contains(x.Id))
                .ToList();

            return tasks.Any(x => !x.IsArchived && x.IsCompleted);
        }

        #endregion

        #region Archiving items

        private async void ArchiveItem(ItemModel item)
        {
            item.IsArchived = true;
            await db.Update(item);
        }

        #endregion

        #region Editing items

        private void ShowModal(ItemModel item)
        {
            var pm = new ModalParameters();
            pm.Add("Modal", item);
            ActiveItem = item;

            ms.OnClose += ModalEditClose;
            ms.Show<AddItem>(pm);
        }

        private async void ModalEditClose(ModalResult result)
        {
            ms.OnClose -= ModalEditClose;

            if (result.Success)
            {
                await db.Update((ItemModel)result.Data);
                Update();
            }
        }

        #endregion

        #region Adding Items

        private void ShowModal()
        {
            ms.OnClose += ModalClosed;
            ms.Show<AddItem>();
        }

        private void ModalClosed(ModalResult result)
        {
            ms.OnClose -= ModalClosed;

            if (result.Success)
            {
                AddItem((ItemModel)result.Data);
            }
        }

        private async void AddItem(ItemModel item)
        {
            if (await db.Create(item))
            {
                ItemAdded?.Invoke(item);
            }
        }

        #endregion

        #region Manager handlers
        private async void TaskAdded(TaskModel task)
        {
            ActiveItem.TaskIds.Add(task.Id);
            await db.Update(ActiveItem);
            Update();
        }

        #endregion


    }
}
