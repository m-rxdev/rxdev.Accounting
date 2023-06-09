using System;

namespace rxdev.Accounting.App.Adapters;

public abstract class EntityAdapter
    : Adapter
{
    private DateTime _entityCreationDate;
    private DateTime _entityEditionDate;
    private int _id;

    public DateTime EntityCreationDate { get => _entityCreationDate; set => Set(ref _entityCreationDate, value); }
    public DateTime EntityEditionDate { get => _entityEditionDate; set => Set(ref _entityEditionDate, value); }
    public int Id { get => _id; set => Set(ref _id, value); }
}