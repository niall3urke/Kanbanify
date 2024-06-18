using Kanbanify.Models;
using Kanbanify.Services;
using Kanbanify.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.ViewModels
{

    public interface IStageManager
    {
        List<StageModel> Stages { get; set; }

        List<ItemModel> Items { get; set; }

        Action<StageModel> ItemStageChanged { get; set; }

        Action<StageModel> StageAdded { get; set; }

        Action StateHasChanged { get; set; }

        void MakeCompletedStage(StageModel stage);

        void ArchiveAllItems(StageModel stage);

        void ItemDropped(StageModel stage);

        void MoveAllItems(StageModel from, StageModel to);

        void Archive(StageModel stage);

        void Add();
    }

    public class StageManager : IStageManager
    {

        // =================
        // ===== Properties
        // =================

        public List<StageModel> Stages { get; set; }

        public List<ItemModel> Items { get; set; }

        // ==============
        // ===== Actions
        // ==============

        public Action<StageModel> ItemStageChanged { get; set; }

        public Action<StageModel> StageAdded { get; set; }

        public Action StateHasChanged { get; set; }

        // =============
        // ===== Fields
        // =============

        private readonly DatabaseOperations db;

        private readonly ModalService ms;

        // ===================
        // ===== Constructors
        // ===================

        public StageManager(DatabaseOperations db, ModalService ms)
        {
            this.db = db;
            this.ms = ms;
        }

        // ========================
        // ===== Interface methods
        // ========================

        public void MakeCompletedStage(StageModel stage)
        {
            RemoveExistingCompletedStage();
            SetStageAsComplete(stage);
            SetItemsAsComplete(stage);
            Update();
        }

        public void ItemDropped(StageModel stage)
        {
            ItemStageChanged?.Invoke(stage);
        }

        public void ArchiveAllItems(StageModel stage)
        {
            ArchiveItems(stage);
            Update();
        }

        public void MoveAllItems(StageModel from, StageModel to)
        {
            MoveItems(from, to);
            Update();
        }

        public void Archive(StageModel stage)
        {
            ArchiveStage(stage);
            ArchiveItems(stage);
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

        #region Set Completed Stage

        private async void RemoveExistingCompletedStage()
        {
            var existing = Stages
                .Where(x => x.IsCompletedStage)
                .FirstOrDefault();

            if (existing != default)
            {
                existing.IsCompletedStage = false;
                await db.Update(existing);
            }
        }

        private async void SetStageAsComplete(StageModel stage)
        {
            stage.IsCompletedStage = true;
            await db.Update(stage);
        }

        private async void SetItemsAsComplete(StageModel stage)
        {
            var items = Items
                .Where(x => x.StageId == stage.Id)
                .ToList();

            foreach (var item in items)
            {
                item.IsCompleted = true;
                await db.Update(item);
            }
        }

        #endregion


        #region Archiving

        private async void ArchiveStage(StageModel stage)
        {
            stage.IsArchived = true;
            await db.Update(stage);
        }

        private async void ArchiveItems(StageModel stage)
        {
            var items = Items
                .Where(x => x.StageId == stage.Id)
                .ToList();

            foreach (var item in items)
            {
                item.IsArchived = true;
                Items.Remove(item);
            }

            await db.Update(items);
        }

        #endregion


        #region Move All Items

        private async void MoveItems(StageModel from, StageModel to)
        {
            var items = Items
                .Where(x => x.StageId == from.Id)
                .ToList();

            foreach (var item in items)
            {
                item.StageId = to.Id;
            }

            await db.Update(items);
        }

        #endregion


        #region Adding Stages

        private void ShowModal()
        {
            ms.OnClose += ModalClosed;
            ms.Show<AddStage>();
        }

        private void ModalClosed(ModalResult result)
        {
            ms.OnClose -= ModalClosed;

            if (result.Success)
            {
                AddStage((StageModel)result.Data);
            }
        }

        private async void AddStage(StageModel stage)
        {
            if (await db.Create(stage))
            {
                StageAdded?.Invoke(stage);
            }
        }

        #endregion 


    }
}
