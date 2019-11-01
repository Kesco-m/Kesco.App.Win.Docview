namespace Kesco.App.Win.DocView.Common
{
    /// <summary>
    /// Параметры отображения
    /// </summary>
    public sealed class ViewParameters
    {
        /// <summary>
        /// Тип изображения
        /// </summary>
        public bool IsPdf { get; set; }

        /// <summary>
        /// Идентификатор изображения
        /// </summary>
        public int ImageId { get; set; }

        /// <summary>
        /// Страница изображения
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Значение Value скролинга относительно Max value (100% - 1, 0% - 0)
        /// </summary>
        public double ActualImageVerticalScrollValue { get; set; }

        /// <summary>
        /// Значение Value скролинга относительно Max value (100% - 1, 0% - 0)
        /// </summary>
        public double ActualImageHorizontalScrollValue { get; set; }

        /// <summary>
        /// Позиция скроллинга
        /// </summary>
        public int ScrollPositionY { get; set; }

        /// <summary>
        /// Позиция скроллинга
        /// </summary>
        public int ScrollPositionX { get; set; }
    }
}