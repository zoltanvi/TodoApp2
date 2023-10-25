using TodoApp2.Core;

namespace TodoApp2
{
    public class DragDropMediator
    {
        private readonly TaskListService _taskListService;
        private readonly CustomDropHandler _customDropHandler;

        public DragDropMediator()
        {
            _taskListService = IoC.TaskListService;
            _customDropHandler = VML.CustomDropHandler;
            _customDropHandler.BeforeItemDropped += OnBeforeItemDropped;
        }

        private void OnBeforeItemDropped(object sender, DragDropEventArgs e)
        {
            int beforeAlteredIndex = e.NewIndex;
            int afterAlteredIndex = _taskListService.GetCorrectReorderIndex(
                e.NewIndex, e.Item as TaskViewModel);

            if (beforeAlteredIndex != afterAlteredIndex)
            {
                _customDropHandler.AlterInsertIndex = true;
                _customDropHandler.NewInsertIndex = afterAlteredIndex;
            }
        }
    }
}
