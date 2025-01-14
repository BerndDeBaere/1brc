﻿using System.Diagnostics;

namespace _1brc;

public static class Program
{
    public static void Main()
    {
        const string file = @"D:\Ex\1brc\measurements-1000000000.txt";
        Console.WriteLine("Start");
        var watch = new Stopwatch();
        watch.Start();
        var blocks = CalculateBlocks(new FileInfo(file).Length, 1024 * 1024 * 1024);
        Parallel.ForEach(blocks, (block) =>
            {
                BlockProcessor processor = new BlockProcessor(block);
                using var stream = File.OpenRead(file);
                stream.Seek(block.From, SeekOrigin.Current);
                var data = (new Byte[1024 * 1024]).AsSpan();
                while (block.AlreadyRead < block.Size)
                {
                    var readLenght = stream.Read(data);
                    processor.Process(data);
                    block.AlreadyRead += readLenght;
                }
            }
        );
        var data = blocks.OrderBy(x => x.Order).SelectMany(x => x.Trim).ToArray().AsSpan();
        var lastBlock = new Block(1, 0, data.Length);
        BlockProcessor processor = new BlockProcessor(lastBlock);
        processor.Process(data, true);

        Dictionary<byte[], CityResult> finalResult = lastBlock.Results;
        foreach (var block in blocks)
        {
            finalResult.CombineDictionary(block.Results);
        }

        string output ="{" + string.Join(", ", finalResult.OrderBy(x => x.Key, new Comparere()).Select(x => x.Value.ToString())) + "}\n";
        Console.WriteLine($"Done {watch.ElapsedMilliseconds}");
        Console.WriteLine(output == Expected? "Woop woop!": "Nope nope");
        Console.Read();
    }

    private class Comparere : IComparer<byte[]>
    {
        public int Compare(byte[]? x, byte[]? y)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (i >= y.Length) return 1;
                var diff = x[i] - y[i];
                if (diff != 0) return diff;
            }

            return 0;
        }
    }

    static Block[] CalculateBlocks(long lenght, long blockSize)
    {
        var numberOfBlocks = (int)Math.Ceiling(lenght / (decimal)blockSize);
        Block[] blocks = new Block[numberOfBlocks];
        blocks[0] = new Block(0, 0, long.Min(lenght, blockSize));

        long starting = blockSize;
        for (int i = 1; i < numberOfBlocks - 1; i++)
        {
            blocks[i] = new Block(i, starting, blockSize);
            starting += blockSize;
        }

        if (numberOfBlocks > 1)
            blocks[^1] = new Block(numberOfBlocks - 1, starting, lenght - starting);
        return blocks;
    }

    private const string Expected = "{Abha=-37.8/18.0/70.8, Abidjan=-25.9/26.0/73.3, Abéché=-19.3/29.4/79.5, Accra=-22.1/26.4/75.6, Addis Ababa=-33.3/16.0/67.1, Adelaide=-33.4/17.3/68.5, Aden=-19.2/29.1/78.7, Ahvaz=-24.0/25.4/73.2, Albuquerque=-32.5/14.0/63.5, Alexandra=-38.0/11.0/61.2, Alexandria=-31.0/20.0/69.0, Algiers=-29.1/18.2/66.7, Alice Springs=-28.5/21.0/77.3, Almaty=-41.8/10.0/58.6, Amsterdam=-40.9/10.2/57.8, Anadyr=-62.6/-6.9/41.4, Anchorage=-45.1/2.8/54.3, Andorra la Vella=-36.3/9.8/58.9, Ankara=-39.9/12.0/65.0, Antananarivo=-31.7/17.9/72.3, Antsiranana=-23.2/25.2/74.4, Arkhangelsk=-50.6/1.3/52.4, Ashgabat=-34.5/17.1/66.7, Asmara=-32.5/15.6/65.3, Assab=-19.9/30.5/83.0, Astana=-45.7/3.5/54.7, Athens=-29.0/19.2/69.8, Atlanta=-30.7/17.0/68.3, Auckland=-34.1/15.2/62.8, Austin=-27.0/20.7/67.8, Baghdad=-27.3/22.8/70.9, Baguio=-33.2/19.5/69.9, Baku=-35.5/15.1/64.3, Baltimore=-37.5/13.1/62.4, Bamako=-22.8/27.8/80.9, Bangkok=-24.5/28.6/79.1, Bangui=-23.8/26.0/75.9, Banjul=-24.2/26.0/73.6, Barcelona=-37.5/18.2/74.0, Bata=-26.5/25.1/72.6, Batumi=-36.8/14.0/60.1, Beijing=-35.4/12.9/62.9, Beirut=-24.5/20.9/72.5, Belgrade=-40.8/12.5/63.7, Belize City=-26.7/26.7/76.7, Benghazi=-27.9/19.9/68.8, Bergen=-48.8/7.7/58.2, Berlin=-39.5/10.3/57.9, Bilbao=-33.2/14.7/62.7, Birao=-24.3/26.5/79.2, Bishkek=-45.7/11.3/67.7, Bissau=-21.1/27.0/74.7, Blantyre=-27.1/22.2/72.2, Bloemfontein=-42.0/15.6/63.0, Boise=-40.5/11.4/62.5, Bordeaux=-32.7/14.2/62.5, Bosaso=-21.9/30.0/84.5, Boston=-40.5/10.9/58.8, Bouaké=-22.8/26.0/78.6, Bratislava=-44.7/10.5/60.2, Brazzaville=-22.9/25.0/74.6, Bridgetown=-23.3/27.0/77.5, Brisbane=-26.0/21.4/74.7, Brussels=-37.6/10.5/61.6, Bucharest=-41.1/10.8/60.5, Budapest=-40.6/11.3/64.5, Bujumbura=-28.0/23.8/73.9, Bulawayo=-30.8/18.9/67.5, Burnie=-35.6/13.1/65.1, Busan=-37.7/15.0/65.8, Cabo San Lucas=-25.6/23.9/73.1, Cairns=-27.8/25.0/76.4, Cairo=-28.8/21.4/70.3, Calgary=-44.4/4.4/54.4, Canberra=-35.5/13.1/60.8, Cape Town=-37.6/16.2/68.1, Changsha=-33.9/17.4/67.2, Charlotte=-35.9/16.1/67.7, Chiang Mai=-25.3/25.8/74.7, Chicago=-44.2/9.8/62.3, Chihuahua=-28.5/18.6/70.5, Chittagong=-27.7/25.9/74.4, Chișinău=-41.6/10.2/58.1, Chongqing=-29.3/18.6/69.3, Christchurch=-41.2/12.2/57.9, City of San Marino=-36.7/11.8/62.2, Colombo=-19.8/27.4/81.5, Columbus=-35.4/11.7/59.9, Conakry=-22.0/26.4/74.4, Copenhagen=-42.2/9.1/60.5, Cotonou=-20.3/27.2/82.0, Cracow=-44.8/9.3/58.8, Da Lat=-33.5/17.9/65.9, Da Nang=-22.7/25.8/78.0, Dakar=-29.9/24.0/72.5, Dallas=-28.4/19.0/71.4, Damascus=-30.7/17.0/66.3, Dampier=-22.9/26.4/77.5, Dar es Salaam=-23.2/25.8/75.5, Darwin=-21.3/27.6/78.3, Denpasar=-27.6/23.7/74.7, Denver=-38.7/10.4/57.7, Detroit=-38.6/10.0/59.7, Dhaka=-25.9/25.9/78.5, Dikson=-63.2/-11.1/38.4, Dili=-24.8/26.6/74.2, Djibouti=-20.4/29.9/83.6, Dodoma=-30.7/22.7/71.8, Dolisie=-24.7/24.0/77.7, Douala=-23.8/26.7/74.6, Dubai=-23.7/26.9/76.3, Dublin=-39.6/9.8/56.5, Dunedin=-39.3/11.1/62.0, Durban=-29.1/20.6/72.9, Dushanbe=-33.5/14.7/66.1, Edinburgh=-42.9/9.3/60.2, Edmonton=-47.9/4.2/54.9, El Paso=-29.6/18.1/66.2, Entebbe=-31.8/21.0/69.1, Erbil=-29.8/19.5/66.8, Erzurum=-43.7/5.1/52.7, Fairbanks=-51.5/-2.3/43.6, Fianarantsoa=-36.5/17.9/67.3, Flores,  Petén=-25.0/26.4/75.6, Frankfurt=-41.1/10.6/57.2, Fresno=-36.4/17.9/68.9, Fukuoka=-31.9/17.0/67.4, Gaborone=-29.5/21.0/72.6, Gabès=-34.8/19.5/69.8, Gagnoa=-28.0/26.0/74.7, Gangtok=-32.5/15.2/66.0, Garissa=-18.0/29.3/81.9, Garoua=-22.6/28.3/81.4, George Town=-21.5/27.9/74.6, Ghanzi=-30.2/21.4/72.3, Gjoa Haven=-61.3/-14.4/34.3, Guadalajara=-27.9/20.9/73.4, Guangzhou=-26.8/22.4/75.4, Guatemala City=-31.0/20.4/69.3, Halifax=-43.0/7.5/63.4, Hamburg=-40.9/9.7/63.0, Hamilton=-43.3/13.8/66.7, Hanga Roa=-32.1/20.5/71.3, Hanoi=-25.0/23.6/73.2, Harare=-34.8/18.4/69.1, Harbin=-43.7/5.0/56.7, Hargeisa=-27.6/21.7/72.3, Hat Yai=-22.1/27.0/77.1, Havana=-24.7/25.2/77.2, Helsinki=-44.6/5.9/63.4, Heraklion=-38.7/18.9/68.1, Hiroshima=-33.7/16.3/64.5, Ho Chi Minh City=-29.3/27.4/75.7, Hobart=-38.8/12.7/61.9, Hong Kong=-33.3/23.3/72.3, Honiara=-21.9/26.5/78.8, Honolulu=-27.8/25.4/78.7, Houston=-29.1/20.8/74.4, Ifrane=-40.6/11.4/62.8, Indianapolis=-38.3/11.8/62.8, Iqaluit=-61.9/-9.3/38.3, Irkutsk=-48.5/1.0/52.4, Istanbul=-35.3/13.9/64.9, Jacksonville=-27.8/20.3/70.9, Jakarta=-23.7/26.7/78.1, Jayapura=-21.7/27.0/82.0, Jerusalem=-29.8/18.3/67.6, Johannesburg=-32.5/15.5/63.7, Jos=-27.2/22.8/74.8, Juba=-23.2/27.8/79.9, Kabul=-42.2/12.1/59.0, Kampala=-27.1/20.0/74.0, Kandi=-25.3/27.7/76.5, Kankan=-23.0/26.5/77.7, Kano=-23.7/26.4/79.2, Kansas City=-39.6/12.5/62.1, Karachi=-25.2/26.0/73.6, Karonga=-23.5/24.4/74.6, Kathmandu=-35.2/18.3/68.4, Khartoum=-21.1/29.9/83.7, Kingston=-21.0/27.4/75.8, Kinshasa=-25.2/25.3/74.1, Kolkata=-25.0/26.7/77.6, Kuala Lumpur=-21.8/27.3/83.7, Kumasi=-23.9/26.0/79.6, Kunming=-32.1/15.7/66.2, Kuopio=-45.9/3.4/55.4, Kuwait City=-23.8/25.7/72.7, Kyiv=-45.5/8.4/55.8, Kyoto=-30.9/15.8/64.4, La Ceiba=-29.8/26.2/75.9, La Paz=-25.3/23.7/77.5, Lagos=-25.0/26.8/76.6, Lahore=-27.6/24.3/73.9, Lake Havasu City=-26.2/23.7/71.6, Lake Tekapo=-40.3/8.7/58.2, Las Palmas de Gran Canaria=-30.7/21.2/69.0, Las Vegas=-27.9/20.3/69.4, Launceston=-35.6/13.1/63.0, Lhasa=-42.4/7.6/59.1, Libreville=-23.9/25.9/76.6, Lisbon=-31.7/17.5/66.0, Livingstone=-25.5/21.8/70.9, Ljubljana=-41.8/10.9/62.0, Lodwar=-20.3/29.3/79.1, Lomé=-26.6/26.9/77.3, London=-43.8/11.3/60.5, Los Angeles=-28.8/18.6/66.6, Louisville=-33.9/13.9/63.6, Luanda=-27.6/25.8/74.9, Lubumbashi=-31.3/20.8/74.0, Lusaka=-28.5/19.9/68.0, Luxembourg City=-41.7/9.3/56.0, Lviv=-46.9/7.8/56.7, Lyon=-34.7/12.5/61.6, Madrid=-34.2/15.0/65.0, Mahajanga=-27.2/26.3/82.0, Makassar=-24.2/26.7/76.6, Makurdi=-22.6/26.0/74.0, Malabo=-21.4/26.3/75.6, Malé=-21.2/28.0/77.0, Managua=-21.2/27.3/74.7, Manama=-28.3/26.5/75.7, Mandalay=-22.4/28.0/78.0, Mango=-20.3/28.1/77.2, Manila=-23.1/28.4/80.1, Maputo=-32.9/22.8/74.1, Marrakesh=-30.3/19.6/70.6, Marseille=-38.0/15.8/70.2, Maun=-25.9/22.4/70.8, Medan=-25.0/26.5/73.0, Mek'ele=-26.4/22.7/75.9, Melbourne=-36.0/15.1/62.5, Memphis=-34.5/17.2/68.5, Mexicali=-26.5/23.1/75.0, Mexico City=-35.2/17.5/67.3, Miami=-24.5/24.9/72.2, Milan=-37.3/13.0/63.7, Milwaukee=-40.5/8.9/58.1, Minneapolis=-42.6/7.8/54.6, Minsk=-41.9/6.7/53.5, Mogadishu=-19.8/27.1/74.6, Mombasa=-21.6/26.3/76.4, Monaco=-32.1/16.4/71.9, Moncton=-46.8/6.1/55.8, Monterrey=-25.2/22.3/82.1, Montreal=-44.7/6.8/55.2, Moscow=-45.4/5.8/58.1, Mumbai=-23.7/27.1/77.1, Murmansk=-52.5/0.6/50.2, Muscat=-20.6/28.0/82.2, Mzuzu=-35.0/17.7/67.5, N'Djamena=-25.9/28.3/77.2, Naha=-27.1/23.1/75.6, Nairobi=-33.2/17.8/65.3, Nakhon Ratchasima=-24.2/27.3/74.1, Napier=-38.0/14.6/65.1, Napoli=-32.0/15.9/67.3, Nashville=-34.7/15.4/66.6, Nassau=-23.5/24.6/75.3, Ndola=-31.1/20.3/67.2, New Delhi=-22.8/25.0/76.3, New Orleans=-27.1/20.7/75.8, New York City=-34.5/12.9/61.7, Ngaoundéré=-31.9/22.0/71.5, Niamey=-25.5/29.3/77.2, Nicosia=-28.3/19.7/72.9, Niigata=-36.5/13.9/62.1, Nouadhibou=-28.6/21.3/72.7, Nouakchott=-24.3/25.7/74.2, Novosibirsk=-46.4/1.7/56.0, Nuuk=-54.6/-1.4/47.5, Odesa=-40.0/10.7/58.2, Odienné=-21.6/26.0/75.7, Oklahoma City=-36.7/15.9/67.9, Omaha=-40.7/10.6/62.7, Oranjestad=-22.0/28.1/79.4, Oslo=-46.6/5.7/53.6, Ottawa=-41.8/6.6/56.9, Ouagadougou=-24.3/28.3/77.7, Ouahigouya=-25.5/28.6/85.6, Ouarzazate=-31.9/18.9/68.8, Oulu=-46.4/2.7/49.8, Palembang=-25.3/27.3/78.6, Palermo=-27.9/18.5/69.3, Palm Springs=-26.4/24.5/77.0, Palmerston North=-43.0/13.2/63.2, Panama City=-21.6/28.0/84.3, Parakou=-22.6/26.8/77.0, Paris=-37.8/12.3/64.0, Perth=-32.3/18.7/68.5, Petropavlovsk-Kamchatsky=-47.5/1.9/52.6, Philadelphia=-36.9/13.2/69.1, Phnom Penh=-24.0/28.3/77.4, Phoenix=-25.0/23.9/74.7, Pittsburgh=-37.7/10.8/62.0, Podgorica=-36.3/15.3/64.3, Pointe-Noire=-23.0/26.1/74.2, Pontianak=-22.1/27.7/76.9, Port Moresby=-21.4/26.9/76.0, Port Sudan=-21.8/28.4/76.0, Port Vila=-24.6/24.3/74.0, Port-Gentil=-22.9/26.0/73.6, Portland (OR)=-36.1/12.4/62.3, Porto=-35.8/15.7/63.0, Prague=-41.3/8.4/61.8, Praia=-27.9/24.4/74.8, Pretoria=-29.9/18.2/69.0, Pyongyang=-38.0/10.8/60.6, Rabat=-31.0/17.2/65.4, Rangpur=-28.4/24.4/76.6, Reggane=-22.0/28.3/84.1, Reykjavík=-45.2/4.3/57.1, Riga=-42.8/6.2/56.8, Riyadh=-26.1/26.0/81.3, Rome=-36.9/15.2/63.9, Roseau=-24.5/26.2/77.9, Rostov-on-Don=-43.4/9.9/58.2, Sacramento=-32.9/16.3/66.1, Saint Petersburg=-45.0/5.8/56.4, Saint-Pierre=-45.0/5.7/58.6, Salt Lake City=-36.5/11.6/59.5, San Antonio=-29.9/20.8/72.3, San Diego=-29.1/17.8/66.7, San Francisco=-34.2/14.6/70.2, San Jose=-33.6/16.4/69.7, San José=-27.1/22.6/70.9, San Juan=-24.3/27.2/76.0, San Salvador=-27.0/23.1/70.9, Sana'a=-31.4/20.0/70.3, Santo Domingo=-22.9/25.9/74.5, Sapporo=-42.0/8.9/59.5, Sarajevo=-42.4/10.1/63.2, Saskatoon=-55.6/3.3/55.8, Seattle=-38.7/11.3/59.1, Seoul=-35.2/12.5/62.9, Seville=-31.6/19.2/68.2, Shanghai=-36.9/16.7/63.8, Singapore=-20.5/27.0/75.4, Skopje=-35.5/12.4/61.1, Sochi=-42.1/14.2/66.9, Sofia=-42.5/10.6/63.0, Sokoto=-23.3/28.0/79.5, Split=-35.0/16.1/65.3, St. John's=-42.8/5.0/55.5, St. Louis=-39.2/13.9/65.0, Stockholm=-52.3/6.6/56.8, Surabaya=-22.2/27.1/75.8, Suva=-22.2/25.6/75.8, Suwałki=-44.7/7.2/62.5, Sydney=-32.8/17.7/66.9, Ségou=-19.3/28.0/85.9, Tabora=-27.3/23.0/73.0, Tabriz=-37.8/12.6/62.5, Taipei=-27.4/23.0/74.6, Tallinn=-40.1/6.4/56.5, Tamale=-20.8/27.9/77.1, Tamanrasset=-34.3/21.7/72.8, Tampa=-29.0/22.9/70.7, Tashkent=-33.9/14.8/67.5, Tauranga=-34.1/14.8/66.6, Tbilisi=-36.2/12.9/62.3, Tegucigalpa=-30.0/21.7/73.8, Tehran=-34.5/17.0/65.9, Tel Aviv=-31.8/20.0/73.4, Thessaloniki=-37.0/16.0/68.9, Thiès=-24.7/24.0/76.0, Tijuana=-30.6/17.8/64.9, Timbuktu=-19.6/28.0/75.3, Tirana=-36.8/15.2/68.2, Toamasina=-31.7/23.4/72.8, Tokyo=-35.2/15.4/68.1, Toliara=-25.7/24.1/71.0, Toluca=-35.6/12.4/61.4, Toronto=-40.7/9.4/60.3, Tripoli=-33.2/20.0/65.0, Tromsø=-46.7/2.9/49.1, Tucson=-32.9/20.9/72.6, Tunis=-30.3/18.4/69.7, Ulaanbaatar=-51.6/-0.4/51.1, Upington=-29.5/20.4/70.7, Vaduz=-39.2/10.1/61.3, Valencia=-31.2/18.3/73.7, Valletta=-30.8/18.8/71.2, Vancouver=-37.1/10.4/60.0, Veracruz=-24.4/25.4/71.8, Vienna=-41.5/10.4/57.9, Vientiane=-33.6/25.9/73.9, Villahermosa=-22.1/27.1/79.1, Vilnius=-45.5/6.0/52.8, Virginia Beach=-35.3/15.8/65.9, Vladivostok=-40.6/4.9/58.7, Warsaw=-48.5/8.5/60.0, Washington, D.C.=-35.4/14.6/63.7, Wau=-18.9/27.8/83.4, Wellington=-35.4/12.9/61.0, Whitehorse=-50.4/-0.1/49.5, Wichita=-32.7/13.9/62.9, Willemstad=-20.4/28.0/74.8, Winnipeg=-45.8/3.0/56.9, Wrocław=-41.9/9.6/58.4, Xi'an=-39.3/14.1/63.3, Yakutsk=-58.5/-8.8/41.3, Yangon=-21.8/27.5/83.6, Yaoundé=-30.1/23.8/72.9, Yellowknife=-56.4/-4.3/47.9, Yerevan=-40.2/12.4/62.4, Yinchuan=-37.7/9.0/61.0, Zagreb=-49.7/10.7/59.4, Zanzibar City=-23.2/26.0/77.2, Zürich=-42.8/9.3/62.1, Ürümqi=-42.4/7.4/57.3, İzmir=-33.2/17.9/65.7}\n";
}