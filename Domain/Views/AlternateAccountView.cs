namespace Domain.Views
{
    public class AlternateAccountView
    {
        public string AlternateName { get; set; }
        public string OpGgUrlLink { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AlternateAccountView view &&
                   AlternateName == view.AlternateName &&
                   OpGgUrlLink == view.OpGgUrlLink;
        }

        protected bool Equals(AlternateAccountView other)
        {
            return string.Equals(AlternateName, other.AlternateName) && string.Equals(OpGgUrlLink, other.OpGgUrlLink);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AlternateName != null ? AlternateName.GetHashCode() : 0) * 397) ^ (OpGgUrlLink != null ? OpGgUrlLink.GetHashCode() : 0);
            }
        }
    }
}
