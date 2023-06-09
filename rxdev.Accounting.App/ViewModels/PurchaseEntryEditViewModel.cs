using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using rxdev.Accounting.App.Adapters;
using rxdev.Accounting.App.Resources.MVVM;
using rxdev.Accounting.Model;
using rxdev.Accounting.Persistence;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace rxdev.Accounting.App.ViewModels;

public class PurchaseEntryEditViewModel
    : EditViewModel<PurchaseEntry, PurchaseEntryAdapter>
{
    private decimal _maxAmount;
    private ICommand? _uploadCommand;

    public PurchaseEntryEditViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Commands.ActionBar.HasUpload = true;
    }

    public decimal MaxAmount { get => _maxAmount; set => Set(ref _maxAmount, value); }
    public ICommand UploadCommand => _uploadCommand ??= new RelayCommand(OnUpload, CanUpload);

    public override void Load(params object[] args)
    {
        base.Load(args);

        Repository<BankTransaction> bankTransactionRepository = ServiceProvider.GetRequiredService<Repository<BankTransaction>>();
        if (Item.BankTransactionId != 0)
            MaxAmount = bankTransactionRepository.AsQueryable().First(e => e.Id == Item.BankTransactionId).Amount;
    }

    protected override IQueryable<PurchaseEntry> GetQuery(bool tracking = false)
        => base.GetQuery(tracking)
        .Include(e => e.Attachment)
        .ThenInclude(e => e!.EntityData);
    
    protected override void Save()
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        UnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
        Repository<PurchaseEntry> repo = scope.ServiceProvider.GetRequiredService<Repository<PurchaseEntry>>();

        PurchaseEntry? entity = repo.AsQueryable(true).FirstOrDefault(e => e.Id == Item.Id);

        if (entity is null)
        {
            entity = Mapper.Map<PurchaseEntry>(Item);
            repo.Add(entity);
        }
        else
        {
            if (Item.Attachment?.Id == 0 && entity.AttachmentId.HasValue)
                scope.ServiceProvider.GetRequiredService<Repository<Attachment>>()
                    .Remove(entity.AttachmentId.Value);

            Mapper.Map(Item, entity);
            if (Item.Attachment?.Id == 0)
                entity.AttachmentId = 0;
            else
                entity.Attachment = null;
            repo.Update(entity);
        }

        unitOfWork.Save();
        Item.Id = entity.Id;
        Item.IsDirty = false;
    }

    private void OnUpload()
    {
        OpenFileDialog ofd = new()
        {
            Filter = "PDF file|*.pdf",
            Multiselect = false,
        };

        if (ofd.ShowDialog() != true)
            return;

        Item.Attachment = new AttachmentAdapter
        {
            FileName = Path.GetFileName(ofd.FileName),
            EntityData = new EntityData()
            {
                Data = File.ReadAllBytes(ofd.FileName),
            }
        };
    }

    private bool CanUpload()
        => true;
}