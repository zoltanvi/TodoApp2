using MediatR;

namespace Modules.Common.Views.DragDrop;

public class DragDropMediator
{
    private readonly IMediator _mediator;
    private readonly CustomDropHandler _customDropHandler;

    public DragDropMediator(IMediator mediator)
    {
        _mediator = mediator;
        _customDropHandler = CustomDropHandler.Instance;
        _customDropHandler.BeforeItemDropped += OnBeforeItemDropped;
    }

    private void OnBeforeItemDropped(object sender, DragDropEventArgs e)
    {
        //var beforeAlteredIndex = e.NewIndex;
        ////int afterAlteredIndex = _mediator.Send(new GetTaskReorderIndexQuery() { TaskObject = e.Item }).Result;
        ////int afterAlteredIndex = _taskListService.GetCorrectReorderIndex(e.NewIndex, e.Item as TaskViewModel);

        //if (beforeAlteredIndex != afterAlteredIndex)
        //{
        //    _customDropHandler.AlterInsertIndex = true;
        //    _customDropHandler.NewInsertIndex = afterAlteredIndex;
        //}
    }
}
