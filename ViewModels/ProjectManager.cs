using Kanbanify.Models;
using Kanbanify.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.ViewModels
{

    public interface IProjectManager
    {
        Action Update { get; set; }

        IStageManager StageManager { get; set; }

        IItemManager ItemManager { get; set; }

        List<StageModel> Stages { get; set; }

        List<ItemModel> Items { get; set; }

        List<TaskModel> Tasks { get; set; }

        ProjectModel Project { get; set; }

        ItemModel ActiveItem { get; set; }

        Task Initialize();
    }

    public class ProjectManager : IProjectManager 
    {

        // =================
        // ===== Properties
        // =================

        public Action Update { get; set; }

        public IStageManager StageManager { get; set; }

        public IItemManager ItemManager { get; set; }

        public List<StageModel> Stages { get; set; }

        public List<ItemModel> Items { get; set; }

        public List<TaskModel> Tasks { get; set; }

        public ProjectModel Project { get; set; }

        public ItemModel ActiveItem { get; set; }

        // =============
        // ===== Fields
        // =============

        private readonly DatabaseOperations db;

        private readonly ModalService ms;

        private readonly Guid projectId;

        // ===================
        // ===== Constructors
        // ===================

        public ProjectManager(Guid projectId, DatabaseOperations db, ModalService ms)
        {
            this.projectId = projectId;
            this.db = db;
            this.ms = ms;
        }

        // ========================
        // ===== Interface methods
        // ========================

        public async Task Initialize()
        {
            await GetProject();
            await GetStages();
            await GetItems();
            await GetTasks();

            SetStageManager();
            SetItemManager();
        }

        // ======================
        // ===== Private methods
        // ======================

        #region Fetching objects from database

        private async Task GetProject()
        {
            var (success, project) = await db.Read<ProjectModel>(projectId);

            if (success)
            {
                Project = project;
            }
        }

        private async Task GetStages()
        {
            foreach (var id in Project.StageIds)
            {
                var (success, stage) = await db.Read<StageModel>(id);

                if (!success)
                    continue;

                if (stage.IsArchived)
                    continue;

                Stages.Add(stage);
            }
        }

        private async Task GetItems()
        {
            foreach (var id in Project.ItemIds)
            {
                var (success, item) = await db.Read<ItemModel>(id);

                if (!success)
                    continue;

                if (item.IsArchived)
                    continue;

                Items.Add(item);
            }
        }

        private async Task GetTasks()
        {
            foreach (var item in Items)
            {
                foreach (var id in item.TaskIds)
                {
                    var (success, task) = await db.Read<TaskModel>(id);

                    if (!success)
                        continue;

                    if (task.IsArchived)
                        continue;

                    Tasks.Add(task);
                }
            }
        }

        #endregion


        #region Setting managers

        private void SetStageManager()
        {
            StageManager = new StageManager(db, ms)
            {
                Stages = Stages,
                Items = Items
            };
            StageManager.ItemStageChanged += ItemStageChanged;
            StageManager.StateHasChanged += StateHasChanged;
            StageManager.StageAdded += StageAdded;
        }

        private void SetItemManager()
        {
            ItemManager = new ItemManager(db, ms)
            {
                Tasks = Tasks
            };
            ItemManager.StateHasChanged += StateHasChanged;
            ItemManager.ItemAdded += ItemAdded;
        }

        #endregion


        #region Manager handlers

        private async void ItemStageChanged(StageModel stage)
        {
            ActiveItem.IsCompleted = stage.IsCompleted;
            ActiveItem.StageId = stage.Id;
            await db.Update(ActiveItem);
            StateHasChanged();
        }

        private async void StageAdded(StageModel stage)
        {
            Project.StageIds.Add(stage.Id);
            Stages.Add(stage);

            await db.Update(Project);
            StateHasChanged();
        }

        private async void ItemAdded(ItemModel item)
        {
            Project.ItemIds.Add(item.Id);
            Items.Add(item);

            await db.Update(item);
            StateHasChanged();
        }

        private void StateHasChanged()
        {
            Update?.Invoke();
        }

        #endregion

    }
}
