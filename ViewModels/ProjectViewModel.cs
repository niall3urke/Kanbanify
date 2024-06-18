using Kanbanify.Models;
using Kanbanify.Services;
using Kanbanify.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.ViewModels
{

    // Use static properties to track less important state features
    // e.g. modal show/hide completed tasks 

    // ~~~~~~~ Functions

    // Project functions:

    // - Copy
    // - Close
    // - Archive
    // - View Archived

    // Stage functions:

    // - [x] Make completion stage  
    // - [x] Move 
    // - [x] Copy 
    // - [x] Edit
    // -     Sort
    // - [x] Archive
    // - [x] Limit Items

    // Stage item functions:

    // -     Move All Items ?
    // -     Copy All Items ?
    // - [x] Archive All Items 

    // Item functions:

    // - [x] Move
    // - [x] Copy 
    // - [x] Edit
    // - [x] Archive

    // ~~~~~~~ Features

    // Project features:

    // - Description
    // - Background
    // - Search

    // Stage features: 

    // - Description 
    // - [x] Background
    
    // Item features:
    
    // - [x] Background

    public interface IProjectViewModel
    {
        List<StageModel> StageModels { get; set; }
        List<ItemModel> ItemModels { get; set; }
        ItemModel SelectedItem { get; set; }
        Action StateHasChanged { get; set; }

        void OnArchiveAllCardsInStageClick(StageModel stage);
        void OnMarkAsCompletionStageClick(StageModel stage);
        void OnMoveAllCardsInStageClick(StageModel from, StageModel to);
        void OnArchiveStageClick(StageModel stage);
        void OnMoveStageClick(StageModel stage);
        void OnCopyStageClick(StageModel stage);
        void OnEditStageClick(StageModel stage);
        void MoveStage(StageModel stage, int index);
        void OnAddStageClick();

        void MoveItem(ItemModel item, Guid stageId, int index);
        void OnItemStageChange(StageModel stage);
        void OnArchiveItemClick(ItemModel item);
        void OnMoveItemClick(ItemModel item);
        void OnCopyItemClick(ItemModel item);
        void OnAddItemClick(Guid stageId);
        void OnItemClick(ItemModel item);

        Task GetModels();
    }

    public class ProjectViewModel : IProjectViewModel
    {

        // Properties

        public List<Tuple<string, string>> Notifications { get; set; }
        public List<StageModel> StageModels { get; set; }
        public List<ItemModel> ItemModels { get; set; }
        public ProjectModel ProjectModel { get; set; }

        public ItemModel SelectedItem { get; set; }
        public Action StateHasChanged { get; set; }

        // Fields

        private readonly DatabaseOperations db;

        private readonly ModalService ms;

        private readonly Guid projectId;

        // Constructors 

        public ProjectViewModel(Guid projectId, DatabaseOperations db, ModalService ms)
        {
            Notifications = new List<Tuple<string, string>>();
            StageModels = new List<StageModel>();
            ItemModels = new List<ItemModel>();

            this.projectId = projectId;
            this.db = db;
            this.ms = ms;
        }

        // Methods - public 

        public async Task GetModels()
        {
            await GetProject();
            await GetStages();
            await GetItems();
            GetNotifications();

            StateHasChanged?.Invoke();
        }

        public void OnMarkAsCompletionStageClick(StageModel stage) =>
            MarkAsCompletionStage(stage);

        public void OnArchiveStageClick(StageModel stage) =>
            ArchiveStageAndItems(stage);

        public void OnArchiveAllCardsInStageClick(StageModel stage) =>
            ArchiveAllItemsInStage(stage);

        public void OnMoveAllCardsInStageClick(StageModel from, StageModel to) =>
            MoveAllItemsInStage(from, to);

        public void OnArchiveItemClick(ItemModel item) =>
            ArchiveItem(item);
        
        public void OnMoveStageClick(StageModel stage) =>
            MoveStage(stage);

        public void OnCopyStageClick(StageModel stage) =>
            CopyStage(stage);

        public void OnEditStageClick(StageModel stage) =>
            EditStage(stage);

        public void OnMoveItemClick(ItemModel item) =>
            MoveItem(item);

        public void OnCopyItemClick(ItemModel item) =>
            CopyItem(item);

        public void OnItemStageChange(StageModel stage) =>
            ItemStageChange(stage);

        public void OnAddStageClick() => 
            AddStageModal();

        public void OnAddItemClick(Guid stageId) => 
            AddItemModal(stageId);

        public void OnItemClick(ItemModel item) => 
            EditItemModal(item);

        // Methods - database fetches

        private async Task GetProject()
        {
            var (success, project) = await db.Read<ProjectModel>(projectId);

            if (success)
            {
                ProjectModel = project;
            }
        }

        private async Task GetStages()
        {
            if (ProjectModel == null)
                return;

            foreach (var id in ProjectModel.StageIds)
            {
                var (success, stage) = await db.Read<StageModel>(id);

                if (!success)
                    continue;

                if (stage.IsArchived)
                    continue;

                StageModels.Add(stage);
            }
        }

        private async Task GetItems()
        {
            if (ProjectModel == null)
                return;

            foreach (var id in ProjectModel.ItemIds)
            {
                var (success, item) = await db.Read<ItemModel>(id);

                if (!success)
                    continue;

                if (item.IsArchived)
                    continue;

                ItemModels.Add(item);
            }
        }

        private void GetNotifications()
        {
            if (ProjectModel == null)
                return;

            var today = DateTime.Now;

            foreach (var item in ItemModels)
            {
                if (item.IsCompleted || item.IsArchived)
                    continue;

                if (!item.HasDueDate)
                    continue;

                var days = (item.DateDue - today).Days;

                if (days > 0)
                    continue;
                
                if (days == 0)
                    Notifications.Add(Tuple.Create($"{item.Name}", "Due Today"));

                if (days < 0)
                    Notifications.Add(Tuple.Create($"{item.Name}", "Overdue"));
            }
        }

        // Methods - Adding stages modal

        private void AddStageModal()
        {
            ms.OnClose += AddStage_ModalClosed;
            ms.Show<AddStage>();
        }

        private async void AddStage_ModalClosed(ModalResult result)
        {
            ms.OnClose -= AddStage_ModalClosed;

            if (result.Success)
            {
                var stage = (StageModel)result.Data;
                ProjectModel.StageIds.Add(stage.Id);
                StageModels.Add(stage);

                await db.Update(ProjectModel);
                await db.Create(stage);

                StateHasChanged?.Invoke();
            }
        }
        
        // Methods - Adding items modal

        private void AddItemModal(Guid stageId)
        {
            var parameters = new ModalParameters();
            parameters.Add("StageId", stageId);

            ms.OnClose += AddItem_ModalClosed;
            ms.Show<AddItem>(parameters);
        }

        private async void AddItem_ModalClosed(ModalResult result)
        {
            ms.OnClose -= AddItem_ModalClosed;

            if (result.Success)
            {
                var item = (ItemModel)result.Data;
                ProjectModel.ItemIds.Add(item.Id);
                ItemModels.Add(item);

                await db.Update(ProjectModel);
                await db.Create(item);

                StateHasChanged?.Invoke();
            }
        }

        // Methods - Editing item modal

        private void EditItemModal(ItemModel item)
        {
            var parameters = new ModalParameters();
            parameters.Add("Model", item);

            ms.OnClose += EditItem_ModalClose;
            ms.Show<AddItem>(parameters);
        }

        private async void EditItem_ModalClose(ModalResult result)
        {
            ms.OnClose -= EditItem_ModalClose;

            if (!result.Success)
                return;

            if (!await db.Update((ItemModel)result.Data))
                return;

            StateHasChanged?.Invoke();
        }

        // Methods - setting completion and start stages

        private void MarkAsCompletionStage(StageModel stage)
        {
            var items = ItemModels.Where(
                x => x.StageId == stage.Id && !x.IsCompleted).ToList();

            RemoveExistingCompletion();
            SetCurrentCompletion(stage);
            MarkItemsAsCompleted(items);

            StateHasChanged?.Invoke();
        }

        private async void RemoveExistingCompletion()
        {
            var existingStage = StageModels.Where(
                x => x.IsCompletedStage).FirstOrDefault();

            if (existingStage == default)
                return;

            existingStage.IsCompletedStage = false;

            await db.Update(existingStage);
        }

        private async void MarkItemsAsCompleted(List<ItemModel> items)
        {
            foreach (var item in items)
            {
                item.DateCompleted = DateTime.Now;
                item.DateModified = DateTime.Now;
                item.IsCompleted = true;

                await db.Update(item);
            }
        }

        private async void SetCurrentCompletion(StageModel stage)
        {
            foreach (var sm in StageModels)
                sm.IsCompletedStage = false;

            stage.IsCompletedStage = true;

            await db.Update(stage);
        }

        // Methods - item stage changed

        private async void ItemStageChange(StageModel stage)
        {
            if (SelectedItem == null)
                return;

            SelectedItem.DateModified = DateTime.Now;
            SelectedItem.StageId = stage.Id;

            if (stage.IsCompletedStage)
            {
                if (!SelectedItem.IsCompleted)
                {
                    SelectedItem.DateCompleted = DateTime.Now;
                    SelectedItem.IsCompleted = true;
                }
            }
            else
            {
                SelectedItem.IsCompleted = false;
            }

            await db.Update(SelectedItem);

            StateHasChanged?.Invoke();
        }

        // Methods - archiving 

        private void MoveAllItemsInStage(StageModel from, StageModel to)
        {
            var items = ItemModels.Where(x => x.StageId == from.Id).ToList();

            foreach (var item in items)
            {
                item.StageId = to.Id;
            }

            StateHasChanged?.Invoke();
        }

        private void ArchiveStageAndItems(StageModel stage)
        {
            var items = ItemModels.Where(
                x => x.StageId == stage.Id).ToList();

            ArchiveStage(stage);
            ArchiveItems(items);

            StateHasChanged?.Invoke();
        }

        private void ArchiveAllItemsInStage(StageModel stage)
        {
            var items = ItemModels.Where(
                x => x.StageId == stage.Id).ToList();

            ArchiveItems(items);

            StateHasChanged?.Invoke();
        }

        private async void ArchiveStage(StageModel stage)
        {
            StageModels.Remove(stage);
            stage.IsArchived = true;

            await db.Update(stage);
        }

        private async void ArchiveItems(List<ItemModel> items)
        {
            foreach (var item in items)
            {
                ItemModels.Remove(item);
                item.IsArchived = true;

                await db.Update(item);
            }
        }

        private async void ArchiveItem(ItemModel item)
        {
            ItemModels.Remove(item);
            item.IsArchived = true;

            await db.Update(item);

            StateHasChanged?.Invoke();
        }

        private void MoveItem(ItemModel item)
        {
            var parameters = new ModalParameters();
            parameters.Add("Project", this);
            parameters.Add("Model", item);

            ms.OnClose += MoveItem_ModalClose;
            ms.Show<MoveItem>(parameters);
        }

        private async void MoveItem_ModalClose(ModalResult result)
        {
            ms.OnClose -= MoveItem_ModalClose;

            if (!result.Success)
                return;

            if (!await db.Update((ItemModel)result.Data))
                return;

            StateHasChanged?.Invoke();
        }

        private async void CopyItem(ItemModel item)
        {
            // Clone the existing item
            var itemCopy = (ItemModel)item.Clone();

            // Get the existing items tasks, clone and apply to copy
            await foreach (var task in GetItemTasks(item))
            {
                var taskCopy = (TaskModel)task.Clone();

                if (await db.Create(taskCopy))
                {
                    itemCopy.TaskIds.Add(taskCopy.Id);
                }
            }

            // Update the project model item ids
            ProjectModel.ItemIds.Add(itemCopy.Id);

            // Update the list of project items
            ItemModels.Add(itemCopy);

            // Update the project in the database
            await db.Update(ProjectModel);

            // Create the item in the database
            await db.Create(itemCopy);

            // Issue an update
            StateHasChanged?.Invoke();
        }

        private async IAsyncEnumerable<TaskModel> GetItemTasks(ItemModel item)
        {
            foreach (var id in item.TaskIds)
            {
                var (success, task) = await db.Read<TaskModel>(id);

                if (success)
                {
                    yield return task;
                }
            }
        }

        public async void MoveItem(ItemModel item, Guid stageId, int index)
        {
            // Update the stage first
            item.StageId = stageId;

            // Get items associated with the stage 
            var items = ItemModels
                .Where(x => x.StageId == stageId)
                .ToList();

            // Get the item at the filtered list index
            var cItem = items[index];

            // Get the index of the item with respect to the project
            int cIndex = ProjectModel.ItemIds.IndexOf(cItem.Id);

            // Insert to the collection
            ItemModels.Remove(item);
            ItemModels.Insert(cIndex, item);

            // Insert to the Project 
            ProjectModel.ItemIds.Remove(item.Id);
            ProjectModel.ItemIds.Insert(cIndex, item.Id);

            // Save changes
            await db.Update(item);
            await db.Update(ProjectModel);

            // Update the UI
            StateHasChanged?.Invoke();
        }

        private void EditStage(StageModel stage)
        {
            var parameters = new ModalParameters();
            parameters.Add("Model", stage);

            ms.OnClose += EditStage_ModalClose;
            ms.Show<AddStage>(parameters);
        }

        private async void EditStage_ModalClose(ModalResult result)
        {
            ms.OnClose -= EditStage_ModalClose;

            if (!result.Success)
                return;

            if (!await db.Update((StageModel)result.Data))
                return;

            StateHasChanged?.Invoke();
        }

        private void MoveStage(StageModel stage)
        {
            var parameters = new ModalParameters();
            parameters.Add("Project", this);
            parameters.Add("Model", stage);

            ms.OnClose += MoveStage_ModalClose;
            ms.Show<MoveStage>(parameters);
        }

        private void MoveStage_ModalClose(ModalResult result)
        {
            if (result.Success)
            {
                StateHasChanged?.Invoke();
            }
        }

        public async void MoveStage(StageModel stage, int index)
        {
            // Remove and insert the stage 
            StageModels.Remove(stage);
            StageModels.Insert(index, stage);

            // Replicate change in project model
            ProjectModel.StageIds.Remove(stage.Id);
            ProjectModel.StageIds.Insert(index, stage.Id);

            // Save changes
            await db.Update(stage);
            await db.Update(ProjectModel);

            // Update the UI
            StateHasChanged?.Invoke();
        }

        private async void CopyStage(StageModel stage)
        {
            // Get all items associated with the existing stage id
            var items = ItemModels
                .Where(x => x.StageId == stage.Id)
                .ToList();

            // Create a copy of the stage with a new id
            var stageCopy = (StageModel)stage.Clone();

            // Loop the existing items and clone them to the new stage
            foreach (var item in items)
            {
                CopyItemWithParent(item, stageCopy.Id);
            }

            // Update the project model stages
            ProjectModel.StageIds.Add(stageCopy.Id);

            // Add the stage to the model list
            StageModels.Add(stageCopy);

            // Create the stage in the database
            await db.Create(stageCopy);

            // Update the project in the database
            await db.Update(ProjectModel);

            // Issue an update
            StateHasChanged?.Invoke();
        }

        private async void CopyItemWithParent(ItemModel item, Guid stageId)
        {
            // Clone the existing item
            var itemCopy = (ItemModel)item.Clone();

            // Update it's stage id
            itemCopy.StageId = stageId;

            // Get the existing items tasks, clone and apply to copy
            await foreach (var task in GetItemTasks(item))
            {
                var taskCopy = (TaskModel)task.Clone();

                if (await db.Create(taskCopy))
                {
                    itemCopy.TaskIds.Add(taskCopy.Id);
                }
            }

            // Update the project model item ids
            ProjectModel.ItemIds.Add(itemCopy.Id);

            // Update the list of project items
            ItemModels.Add(itemCopy);

            // Create the item in the database
            await db.Create(itemCopy);

            // Update the project in the database
            await db.Update(ProjectModel);
        }
    }
}
