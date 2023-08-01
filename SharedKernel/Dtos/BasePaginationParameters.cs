namespace SharedKernel.Dtos
{
    public abstract class BasePaginationParameters
    {
        internal virtual int MaxPageSize { get; } = 20;
        internal virtual int DefaultPageSize { get; set; } = 10;

        public virtual int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get
            {
                return DefaultPageSize;
            }
            set
            {
                DefaultPageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }
    }
}