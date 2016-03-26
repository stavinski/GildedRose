using System;
using System.Collections.Generic;

namespace GildedRose.Console
{
    public class EndOfDay
    {
        private const int MAX_QUALITY = 50;

        public const string AGED_BRIE = "Aged Brie";
        public const string SULFURAS = "Sulfuras, Hand of Ragnaros";
        public const string BACKSTAGE_PASSES = "Backstage passes to a TAFKAL80ETC concert";

        public IList<Item> Stock { get; }

        public EndOfDay(IList<Item> stock)
        {
            Stock = stock;
        }

        public void UpdateQuality()
        {
            foreach (var stockItem in Stock)
            {
                PreAdjustQuality(stockItem);
                DecreaseSellIn(stockItem);
                PostAdjustQuality(stockItem);
            }
        }
        
        private static void PreAdjustQuality(Item stockItem)
        {
            var increasesInQuality = stockItem.Name == AGED_BRIE || stockItem.Name == BACKSTAGE_PASSES;

            if (increasesInQuality)
            {
                if (stockItem.Quality >= MAX_QUALITY) return;

                stockItem.Quality++;
                BackStagePassQuality(stockItem);
                return;
            }

            bool canDecreaseQuality = stockItem.Quality > 0 && stockItem.Name != SULFURAS;
            if (canDecreaseQuality) stockItem.Quality--;
        }

        private static void BackStagePassQuality(Item stockItem)
        {
            if (stockItem.Name == BACKSTAGE_PASSES)
            {
                if (stockItem.SellIn < 11)
                    stockItem.Quality = Math.Min(stockItem.Quality + 1, MAX_QUALITY);

                if (stockItem.SellIn < 6)
                    stockItem.Quality = Math.Min(stockItem.Quality + 1, MAX_QUALITY);
            }
        }

        private static void DecreaseSellIn(Item stockItem)
        {
            if (stockItem.Name != SULFURAS)
                stockItem.SellIn--;
        }

        private static void PostAdjustQuality(Item stockItem)
        {
            if (stockItem.SellIn >= 0)
                return;

            if (stockItem.Name == SULFURAS)
                return;

            if ((stockItem.Name == AGED_BRIE) && (stockItem.Quality < MAX_QUALITY))
            {
                stockItem.Quality++;
                return;
            }

            if (stockItem.Name == BACKSTAGE_PASSES)
            {
                stockItem.Quality = 0;
            }
            else
            {
                stockItem.Quality = Math.Max(stockItem.Quality - 1, 0);
            }
        }
    }
}
