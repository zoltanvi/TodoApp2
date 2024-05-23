using System.Collections.Generic;

namespace TodoApp2.Core.Services;

public interface ITaskContentSplitterService
{
    List<string> SplitTaskContent(string content);
}
