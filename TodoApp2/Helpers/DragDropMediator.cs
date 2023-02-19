using TodoApp2.Core;

namespace TodoApp2
{
    public class DragDropMediator
    {
        private readonly TaskListService m_TaskListService;
        private readonly CustomDropHandler m_CustomDropHandler;

        public DragDropMediator()
        {
            m_TaskListService = IoC.TaskListService;
            m_CustomDropHandler = ViewModelLocator.CustomDropHandler;
            m_CustomDropHandler.BeforeItemDropped += OnBeforeItemDropped;
        }

        private void OnBeforeItemDropped(object sender, DragDropEventArgs e)
        {
            int beforeAlteredIndex = e.NewIndex;
            int afterAlteredIndex = m_TaskListService.GetCorrectReorderIndex(
                e.NewIndex, e.Item as TaskViewModel);

            if (beforeAlteredIndex != afterAlteredIndex)
            {
                m_CustomDropHandler.AlterInsertIndex = true;
                m_CustomDropHandler.NewInsertIndex = afterAlteredIndex;
            }
        }
    }
}
