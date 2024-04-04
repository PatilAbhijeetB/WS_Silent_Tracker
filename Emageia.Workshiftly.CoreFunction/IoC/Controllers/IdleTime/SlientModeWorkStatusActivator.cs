using Emageia.Workshiftly.CoreFunction.IoC.HalAccess;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.IdleTime
{
    public class SlientModeWorkStatusActivator
    {
        private static int NO_CHECKSUM_SLOTS = 15;
        private static int WORKING_IDLE_TIME_UPPER_BOUND = 10;


        //   private RawDataModule rawDataModule;
        private Int32?[] checksumSlots;
        //   private JsonObject output;

        private int currentSlotId;

        public SlientModeWorkStatusActivator()
        {

            checksumSlots = new Int32?[NO_CHECKSUM_SLOTS];

            ArraysFill(checksumSlots, 0, true);

            currentSlotId = 0;

        }

        internal bool callLister()
        {
            // user current slot id eka
            Int32 userIdleTime = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            var keyInputSinBit = Win32KeyMouseAPICall.GetLastInputTimesTogetSignBit();
            if (currentSlotId == checksumSlots.Length)
            {
                currentSlotId = 0;
            }

            checksumSlots[currentSlotId] = keyInputSinBit == 0 ? 1 : -1;

            //check null value has
            bool isFilledChecksums = checksumSlots.ToList().TrueForAll(x => x.HasValue);

            if (!isFilledChecksums)
            {

                currentSlotId++;
                return false;
            }


            return analyzeCheckSumSlots();

        }

        private bool analyzeCheckSumSlots()
        {
            try
            {
                List<Int32?> checkSumList = checksumSlots.ToList();
                //List<Int32?> checkSumLists = new List<Int32?>(checksumSlots);
                List<Int32?> analyticalList = new List<Int32?>();


                bool isFilledLastSlot = currentSlotId == checksumSlots.Length - 1;

                if (isFilledLastSlot)
                {
                    analyticalList.AddRange(checkSumList);

                }
                else
                {
                    var Sublist = checkSumList.Select((x, i) => new { Index = i, Value = x })
                                            .GroupBy(x => x.Index > currentSlotId)
                                            .Select((x) => x.Select(v => v.Value).ToList())
                                            .ToList();

                    analyticalList = Sublist[1].Concat(Sublist[0]).ToList();
                }


                Int32 maxIdleDuration = analyticalList.Max(x => x.Value);
                

                var CountMouseClick = ConsiderableTimeMouseKeyEvent(analyticalList);
                if (CountMouseClick > 7)
                {
                    
                    CommonUtility.IsIdleDbWrite = true;
                    return true;
                }
                else
                {
                    ArraysFill(checksumSlots, 0, true);
                    currentSlotId = 0;
                    CommonUtility.IsIdleDbWrite = false;
                    
                    return false;
                }


               // currentSlotId++;
            }
            catch (Exception)
            {
                return false;
            }
        }

      
        private int ConsiderableTimeMouseKeyEvent(List<Int32?> checkCountList)
        {
            return checkCountList.FindAll(x => x == 1).Count;
        }

        public void ArraysFill(int?[] array, int Value, bool isNull)
        {
            if (isNull)
            {
                for (Int32 i = 0; i < array.Length; i++)
                {
                    array[i] = null;
                }
                checksumSlots = array;
            }
            else
            {
                for (Int32 i = 0; i < array.Length; i++)
                {
                    array[i] = Value;
                }

            }
        }

    }
}
