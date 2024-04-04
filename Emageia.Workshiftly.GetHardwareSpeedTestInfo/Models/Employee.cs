using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models
{
    public class Employee
    {
        string _EmployeeId = string.Empty;
        string _DeptName = string.Empty;
        string _Deptid = string.Empty;
        string _FirstName = string.Empty;
        string _MiddleName = string.Empty;
        string _LastName = string.Empty;
        string _Designation = string.Empty;
        string _Vertical = string.Empty;
        string _SubVertical = string.Empty;
        string _Grade = string.Empty;
        string _Email = string.Empty;
        string _phone = string.Empty;
        string _location = string.Empty;
        string _Country = string.Empty;
        string _SupervisorName = string.Empty;
        string _Sublocation = string.Empty;
        string _SDPdesc = string.Empty;
        string _SDPstatus = string.Empty;
        string _empStatus = string.Empty;
        string _encryptstr = "꺥㲟ﶘ㕖狀舷ꑒ즯䲫姻鏋盳";
        string _encryptsp = "1234";
        string _salt = "azyxedrknkl";

        public string Location
        {
            set { _location = value; }
            get { return _location; }
        }
        public string Salt
        {

            set { _salt = "azyxedrknkl"; }
            get { return _salt; }
        }
        public string Encryptstr
        {
            set { _encryptstr = "꺥㲟ﶘ㕖狀舷ꑒ즯䲫姻鏋盳"; }
            get { return _encryptstr; }
        }
        public string Encryptsp
        {
            set { _encryptsp = "1234"; }
            get { return _encryptsp; }
        }

        public string SDPDescription
        {
            set { _SDPdesc = value; }
            get { return _SDPdesc; }
        }

        public string SDPStatus
        {
            set { _SDPstatus = value; }
            get { return _SDPstatus; }
        }

        public string Sublocation
        {
            set { _Sublocation = value; }
            get { return _Sublocation; }
        }

        public string Country
        {
            set { _Country = value; }
            get { return _Country; }
        }

        public string Deptid
        {
            set { _Deptid = value; }
            get { return _Deptid; }
        }

        public string DeptName
        {
            set { _DeptName = value; }
            get { return _DeptName; }
        }

        public string SupervisorName
        {
            set { _SupervisorName = value; }
            get { return _SupervisorName; }
        }

        public string EmployeeId
        {
            set { _EmployeeId = value; }
            get { return _EmployeeId; }
        }
        public string FirstName
        {
            set { _FirstName = value; }
            get { return _FirstName; }
        }
        public string MiddleName
        {
            set { _MiddleName = value; }
            get { return _MiddleName; }
        }
        public string LastName
        {
            set { _LastName = value; }
            get { return _LastName; }
        }
        public string Designation
        {
            set { _Designation = value; }
            get { return _Designation; }
        }
        public string Vertical
        {
            set { _Vertical = value; }
            get { return _Vertical; }
        }
        public string SubVertical
        {
            set { _SubVertical = value; }
            get { return _SubVertical; }
        }
        public string Grade
        {
            set { _Grade = value; }
            get { return _Grade; }
        }

        public string Email
        {
            set { _Email = value; }
            get { return _Email; }
        }

        public string phone
        {
            set { _location = value; }
            get { return _location; }
        }
        public string empStatus
        {
            set { _empStatus = value; }
            get { return _empStatus; }
        }
    }
}
