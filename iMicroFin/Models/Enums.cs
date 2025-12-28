namespace iMicroFin.Models
{
    public enum EGender
    {
        Female, TG
    }
    public enum EMaritalStatus
    {
        NA=-1,Married, Seperated, Divorced, Widowed, Unmarried, Untagged
    }
    public enum EReligion
    {
        NA = -1, Hindu, Muslim, Christian
    }
    public enum EMemberType
    {
        Member, Leader
    }
    public enum EHouseType
    {
        NA=-1,ColonyHouse, Thatched, RCC, Tiled
    }
    public enum EPropertyOwnership
    {
        NA = -1, Self, Rented, Other
    }
    public enum EOccupation
    {
        NA = -1, Agriculture, Manufacturing, Service, Trading
    }
    public enum EOccupationType
    {
        NA = -1, SelfEmployed, Salaried, None
    }

    public enum ERelationship
    {
        NA = -1, Father, Husband, Mother, Son, Daughter, Wife, Brother,
        Mother_In_Law, Father_In_Law, Daughter_In_Law,
        Sister_In_Law, Son_In_Law, Brother_In_Law, Other
    }
}