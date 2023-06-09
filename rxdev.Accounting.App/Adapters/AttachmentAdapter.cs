using rxdev.Accounting.Model;

namespace rxdev.Accounting.App.Adapters;

public class AttachmentAdapter
    : EntityAdapter
{
    private string _fileName = string.Empty;

    public string FileName { get => _fileName; set => SetDirty(ref _fileName, value); }
    public EntityData? EntityData { get; set; }
}