namespace ValueCo.CashUpApi.Data;

public static class StoreSeeder
{
    public static async Task SeedAsync(CashUpDbContext db)
    {
        if (db.Stores.Any()) return;
        var stores = new List<ValueCo.CashUpApi.Models.Store>
        {
            new() { StoreCode="VCL01", StoreName="VCL Randburg",          BankAccount="63168708932", SpeedPointId="00627792" },
            new() { StoreCode="VCL02", StoreName="VCL Centurion",         BankAccount="63168708015", SpeedPointId="00627479" },
            new() { StoreCode="VCL03", StoreName="VCL Alberton",          BankAccount="63168707364", SpeedPointId="00626349" },
            new() { StoreCode="VCL04", StoreName="VCL Vaal",              BankAccount="63168707281", SpeedPointId="00628725" },
            new() { StoreCode="VCL05", StoreName="VCL Boksburg",          BankAccount="63168707025", SpeedPointId="00628253" },
            new() { StoreCode="VCL06", StoreName="VCL Atterbury",         BankAccount="63168708792", SpeedPointId="00627149" },
            new() { StoreCode="VCL07", StoreName="VCL Secunda",           BankAccount="63168708594", SpeedPointId="00629327" },
            new() { StoreCode="VCL08", StoreName="VCL North Rand Square", BankAccount="63168707637", SpeedPointId="00630606" },
            new() { StoreCode="VCL09", StoreName="VCL Somerset West",     BankAccount="63168706986", SpeedPointId="00629418" },
            new() { StoreCode="VCL10", StoreName="VCL Blueberry",         BankAccount="63168706861", SpeedPointId="00629442" },
            new() { StoreCode="VCL12", StoreName="VCL The Reef",          BankAccount="63168706712", SpeedPointId="00629152" },
            new() { StoreCode="VCL15", StoreName="VCL Ryneveld",          BankAccount="63168707455", SpeedPointId="00629236" },
            new() { StoreCode="VCL16", StoreName="VCL Horizon View",      BankAccount="63168707174", SpeedPointId="00630366" },
            new() { StoreCode="VCL17", StoreName="VCL Randfontein",       BankAccount="63168707108", SpeedPointId="00630291" },
            new() { StoreCode="VCL18", StoreName="VCL Harvest Place",     BankAccount="63168707546", SpeedPointId="00631430" },
            new() { StoreCode="VCL19", StoreName="VCL Bethlehem",         BankAccount="63168706457", SpeedPointId="00630861" },
            new() { StoreCode="VCL20", StoreName="VCL Lambton Gardens",   BankAccount="63168708510", SpeedPointId="00630408" },
            new() { StoreCode="VCL21", StoreName="VCL Comaro Crossing",   BankAccount="63168708172", SpeedPointId="00629822" },
            new() { StoreCode="VCL22", StoreName="VCL Rand Steam",        BankAccount="63168707736", SpeedPointId="00631711" },
            new() { StoreCode="VCL23", StoreName="VCL Waterfall Ridge",   BankAccount="63168708669", SpeedPointId="00630150" },
            new() { StoreCode="VCL24", StoreName="VCL Wavecrest - Test",  BankAccount="",            SpeedPointId="" },
        };
        db.Stores.AddRange(stores);
        await db.SaveChangesAsync();
    }
}