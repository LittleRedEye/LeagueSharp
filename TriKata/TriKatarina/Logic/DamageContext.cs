namespace TriKatarina.Logic
{
    public class DamageContext
    {
        public double PDamage { get; set; }
        public double QDamage { get; set; }
        public double WDamage { get; set; }
        public double EDamage { get; set; }
        public double RDamage { get; set; }

        public double DFGDamage { get; set; }
        public double HXGDamage { get; set; }
        public double BWCDamage { get; set; }

        public double IgniteDamage { get; set; }
        public double LiandrysDamage { get; set; }

        public double ItemDamage
        {
            get { return DFGDamage + HXGDamage + BWCDamage + LiandrysDamage; }
        }

        public double SkillDamage
        {
            get { return PDamage + QDamage + EDamage + WDamage + RDamage; }
        }

        public double TotalDamage
        {
            get { return ItemDamage + SkillDamage + IgniteDamage; }
        }
    }
}