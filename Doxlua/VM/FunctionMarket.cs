namespace Doxlua.VM
{
    public static class FunctionMarket
    {
        public static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // 0 = OK
        // 1 = Expected Table for table access
        // 2 = Expected String or Number for table access
        // Expected on top of the stack: [TABLE, KEY, ...]
        // Top of the stack after the function: [VALUE, ...]

        public static int TableAccess(DoxState state)
        {
            DoxCell[] arg = state.GetArgs(2);

            if (arg[0].GetDoxType() != DoxValueType.Table)
                return 1;

            if (arg[1].GetDoxType() != DoxValueType.String && arg[1].GetDoxType() != DoxValueType.Number)
                return 2;

            var table = (DoxTable)arg[0];
            var key = arg[1];
            var result = table.Get(key);

            //Console.WriteLine("Table: {0}", table);
            //Console.WriteLine("Key: {0}", key);
            //Console.WriteLine("Result: {0}", result);

            if (result.GetDoxType() == DoxValueType.Nil)
                table.Set(key, result);

            state.Return(result);

            return 0;
        }
        // 0 = OK
        // 1 = Expected Table for table access
        // 2 = Expected String or Number for table access
        // Expected on top of the stack: [TABLE, KEY, VALUE, ...]
        // Top of the stack after the function: [NIL, ...]
        public static int SetTable(DoxState state)
        {
            DoxCell[] arg = state.GetArgs(3);

            if (arg[0].GetDoxType() != DoxValueType.Table)
                return 1;

            if (arg[1].GetDoxType() != DoxValueType.String && arg[1].GetDoxType() != DoxValueType.Number)
                return 2;

            var table = (DoxTable)arg[0];
            var key = arg[1];

            table.Set(key, arg[2]);

            state.Return();

            return 0;
        }

        // 0 = OK
        // 1 = Expected Table for table access
        public static int SetValue(DoxState state)
        {
            DoxCell[] arg = state.GetArgs(2);


            var settee = arg[0];
            var setted = arg[1];

            Logger.Debug("Settee before: {0}", settee);

            settee.SetValue(setted);

            Logger.Debug("Settee after: {0}", settee);
            Logger.Debug("ENV: {0}", state.PeekEnv());


            state.Return();

            return 0;
        }
        public static int PrintFunction(DoxState state)
        {
            DoxCell[] arg = state.GetArgs(1);

            string str = arg[0].GetDoxType() switch
            {
                DoxValueType.Nil => "nil",
                DoxValueType.Boolean => ((DoxBoolean)arg[0]).GetValue().ToString(),
                DoxValueType.Number => ((DoxNumber)arg[0]).GetValue().ToString(),
                DoxValueType.String => ((DoxString)arg[0]).GetValue(),
                _ => "unknown"
            };

            if (str == "unknown")
                return 1;

            Console.WriteLine(str);

            state.Return();

            return 0;
        }


    }

}