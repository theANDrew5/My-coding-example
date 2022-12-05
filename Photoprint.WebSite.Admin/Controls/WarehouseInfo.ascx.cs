using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Photoprint.WebSite.Admin.Controls
{
    public partial class WarehouseInfo : BaseControl
    {
        private Address _address;

        public Address Address
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(adressState.Value))
                {
                    var jObj = JObject.Parse(adressState.Value);
                    _address = jObj.ToObject<Address>() ?? _address;
                }
                _address ??= new Address();
                return _address;
            }
            set => _address = value;
        }

        public string EmployeeName { get; private set; }
        public string EmployeePhone { get; private set; }
        public string EmployeeMail { get; private set; }
        public bool IsMailVisible { get; private set; }
        public string EmployeePhoneClean => Regex.Replace(EmployeePhone, @"[ ,\(,\),-]", "");
        public string EmployeeCompany { get; private set; }
        public bool IsCompanyVisible { get; private set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                EmployeeName = txtEmployeeName.Text;
                EmployeePhone = txtEmployeePhone.Text;
                EmployeeMail = txtEmployeeMail.Text;
                EmployeeCompany = txtEmployeeCompany.Text;
                return;
            }

            txtEmployeeName.Text = EmployeeName;
            txtEmployeePhone.Text = EmployeePhone;
            txtEmployeeMail.Text = EmployeeMail;

            reqMail.Enabled = IsMailVisible;
            reqCompany.Enabled = IsCompanyVisible;

            if (IsMailVisible)
                txtEmployeeMail.Text = EmployeeMail;
            if(IsCompanyVisible)
                txtEmployeeCompany.Text = EmployeeCompany;
        }

        public void InitControl(Address address, (string Name, string Phone, string Mail, string Company) info, ( bool ShowMail, bool ShowCompany) settings)
        {
            Address = address;

            EmployeeName = info.Name;
            EmployeePhone = info.Phone;
            EmployeeMail = info.Mail;
            EmployeeCompany = info.Company;

            IsMailVisible = settings.ShowMail;
            IsCompanyVisible = settings.ShowCompany;
        }
    }
}