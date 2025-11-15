namespace iMicroFin.Models
{
    public enum EGender
    {
        Female, TG
    }
    public enum EMaritalStatus
    {
        Married, Seperated, Divorced, Widowed, Unmarried, Untagged
    }
    public enum EReligion
    {
        Hindu, Muslim, Christian
    }
    public enum EMemberType
    {
        Member, Leader
    }
    public enum EHouseType
    {
        ColonyHouse, Thatched, RCC, Tiled
    }
    public enum EPropertyOwnership
    {
        Self, Rented, Other
    }
    public enum EOccupation
    {
        Agriculture, Manufacturing, Service, Trading
    }
    public enum EOccupationType
    {
        SelfEmployed, Salaried, None
    }

    public enum ERelationship
    {
        Father, Husband, Mother, Son, Daughter, Wife, Brother,
        Mother_In_Law, Father_In_Law, Daughter_In_Law,
        Sister_In_Law, Son_In_Law, Brother_In_Law, Other
    }
}