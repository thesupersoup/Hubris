namespace Hubris
{
    /// <summary>
    /// Holds ports in an int array and provides them sequentially when requested
    /// </summary>
    public class PortPool
    {
        public const int ERROR = -1;

        private int[] portArr;
        private int index;

        public PortPool(int[] nArr = null)
        {
            portArr = nArr;
            index = 0;
        }

        /// <summary>
        /// Get the next port in the array, will loop when the end of the array is reached
        /// </summary>
        /// <returns>Returns port as int</returns>
        public int GetPort()
        {
            int port = ERROR;

            if (portArr != null && portArr.Length > 0)
            {
                port = portArr[index];
                index++;
                if (index == portArr.Length)
                    index = 0;
            }

            return port;
        }

       
    }
}
