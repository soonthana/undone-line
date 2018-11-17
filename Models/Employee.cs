using System;
using System.Collections.Generic;

namespace Undone.Line.BusinessModel
{
  public class Employee
  {
    public string EmployeeId { get; set; }
    public string EmployeeNameEn { get; set; }
    public string EmployeeNameTh { get; set; }
    public string EmployeeSurnameEn { get; set; }
    public string EmployeeSurnameTh { get; set; }
    public string EmployeeStatus { get; set; }
    public string IdentificationNumber { get; set; }
    public string Email { get; set; }
    public string PhoneExtension { get; set; }
    public string TeamCode { get; set; }
    public string TeamNameEn { get; set; }
    public string TeamNameTh { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentNameEn { get; set; }
    public string DepartmentNameTh { get; set; }
    public string JobCode { get; set; }
    public string JobNameEn { get; set; }
    public string JobNameTh { get; set; }
    public string StartWorkDateW3C { get; set; }
    public List<Superordinate> SuperordinateList { get; set; }
    public ImageBase64String ImageBase64Info { get; set; }
    public string ImageUrl { get; set; }
  }

  public class ImageBase64String
  {
    public string ImageBase64 { get; set; }
    public string ImageFileExtension { get; set; }
  }

  public class Superordinate
  {
    public string SuperordinateId { get; set; }
    public string SuperordinateNameEn { get; set; }
    public string SuperordinateNameTh { get; set; }
    public string SuperordinateSurnameEn { get; set; }
    public string SuperordinateSurnameTh { get; set; }
    public string SuperordinateEmail { get; set; }
  }
}