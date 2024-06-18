using Kanbanify.Models;
using Kanbanify.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.ViewModels
{

    public interface ITaskManager
    {
        Action<TaskModel> TaskAdded { get; set; }

        Action StateHasChanged { get; set; }

        void Archive(TaskModel task);

        void Edit(TaskModel task);

        void Add(TaskModel task);
    }

    public class TaskManager : ITaskManager
    {

        // ==============
        // ===== Actions
        // ==============

        public Action<TaskModel> TaskAdded { get; set; }

        public Action StateHasChanged { get; set; }

        // =============
        // ===== Fields
        // =============

        private readonly DatabaseOperations db;

        // ===================
        // ===== Constructors
        // ===================

        public TaskManager(DatabaseOperations db)
        {
            this.db = db;
        }

        // ========================
        // ===== Interface methods
        // ========================

        public void Archive(TaskModel task)
        {
            ArchiveTask(task);
        }

        public void Edit(TaskModel task)
        {

        }

        public async void Add(TaskModel task)
        {
            if (await db.Create(task))
            {
                TaskAdded?.Invoke(task);
            }
        }

        // ======================
        // ===== Private methods
        // ======================

        private void Update() =>
            StateHasChanged?.Invoke();

        #region Archiving tasks

        private async void ArchiveTask(TaskModel task)
        {
            task.IsArchived = true;
            await db.Update(task);
        }

        #endregion


    }
}
