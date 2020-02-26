using System;

namespace Actio.Common.Events
{
  public class CreateActivityRejected : IRejectedEvent
  {
    public Guid Id { get; }
    public string Name { get; }
    public string Reason { get; }
    public string Code { get; }

    protected CreateActivityRejected () { }

    public CreateActivityRejected (Guid id, string name, string reason, string code)
    {
      Id = id;
      Name = name;
      Reason = reason;
      Code = code;
    }
  }
}