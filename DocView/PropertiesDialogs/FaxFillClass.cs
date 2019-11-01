using System;
using System.Data;
using System.Resources;
using Kesco.App.Win.DocView.PropertiesDialogs.FaxItem;
using Kesco.Lib.Win.Data.DALC;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.PropertiesDialogs
{
    public class FaxFillClass
    {
        public static void FaxInFillClass(PropertiesFaxDialog dialog, int faxID)
        {
            var resources = new ResourceManager(typeof (FaxFillClass));
            DataRow dr = Environment.FaxInData.GetFaxIn(faxID);
            if (dr == null)
                return;
            try
            {
                // ������� ���������
                int imageID = 0;
                object obj = dr[Environment.FaxInData.DocImageIDField];
                if (obj != null && !obj.Equals(DBNull.Value))
                    imageID = (int) obj;
                FaxPropertyControl fpc;
                if (imageID > 0)
                {
                    fpc = new FaxPropertiButtonControl("", resources.GetString("FaxSaved"), 2);
                    ((FaxPropertiButtonControl) fpc).FaxID = faxID;
                }
                else
                    fpc = new FaxPropertyControl("", resources.GetString("FaxNotSaved"));
                dialog.PutControl(fpc);

                // ��������
                var read = (bool) dr[Environment.FaxInData.ReadField];
                fpc = new FaxPropertyControl(resources.GetString("Read.Title"),
                                             read ? (resources.GetString("Yes")) : (resources.GetString("No")));
                dialog.PutControl(fpc);

                // ��������
                fpc = new FaxPropertyControl(resources.GetString("Description"),
                                             dr[Environment.FaxInData.DescriptionField].ToString());
                dialog.PutControl(fpc);

                // �������
                var date = (DateTime) dr[Environment.FaxInData.DateField];
                fpc = new FaxPropertyControl(resources.GetString("Date"), ObjectToString.Convert(date.ToLocalTime()));
                dialog.PutControl(fpc);

                // �����������
                fpc = new FaxPropertyControl(resources.GetString("Sender"),
                                             dr[Environment.FaxInData.SenderField].ToString());
                dialog.PutControl(fpc);

                // ����������
                fpc = new FaxPropertyControl(resources.GetString("Recip"),
                                             dr[Environment.FaxInData.RecipField].ToString());
                dialog.PutControl(fpc);

                // CSID
                fpc = new FaxPropertyControl("CSID:", dr[Environment.FaxInData.CSIDField].ToString());
                dialog.PutControl(fpc);

                // ���
                if (dr[Environment.FaxInData.CSIDField].ToString() == dr[Environment.FaxInData.SenderField].ToString() ||
                    dr[Environment.FaxInData.SenderField].ToString() ==
                    dr[Environment.FaxInData.SenderAddressField].ToString())
                {
                    fpc = new FaxPropertiButtonControl(resources.GetString("SenderAddress"),
                                                       dr[Environment.FaxInData.SenderAddressField].ToString(), 1);
                }
                else
                    fpc = new FaxPropertyControl(resources.GetString("SenderAddress"),
                                                 dr[Environment.FaxInData.SenderAddressField].ToString());
                dialog.PutControl(fpc);

                // Modem
                if (dr[Environment.FaxInData.ModemIDField].ToString().Length > 0)
                {
                    fpc = new FaxPropertyControl(resources.GetString("ModemID"),
                                                 dr[Environment.FaxInData.ModemIDField].ToString());
                    dialog.PutControl(fpc);
                }

                // �������� ��������
                string speed = dr[Environment.FaxInData.SpeedField].ToString();
                if (speed.Length > 0)
                {
                    fpc = new FaxPropertyControl(resources.GetString("Speed"), speed + " " + resources.GetString("baud"));
                    dialog.PutControl(fpc);
                }

                // ����� ��������
                string time = dr[Environment.FaxInData.DurationField].ToString();
                if (time.Length > 0)
                {
                    fpc = new FaxPropertyControl(resources.GetString("Time"), time + " " + resources.GetString("Second"));
                    dialog.PutControl(fpc);
                }

                // �������� �������
                if (dr[Environment.FaxInData.PageRecvCountField].ToString().Length > 0)
                {
                    fpc = new FaxPropertyControl(resources.GetString("PageRecvCount"),
                                                 dr[Environment.FaxInData.PageRecvCountField].ToString());
                    dialog.PutControl(fpc);
                }

                // �������
                var empID = (int) dr[Environment.FaxInData.EditorField];
                var emp = new Employee(empID, Environment.EmpData);
                fpc = empID > 0
                          ? new FaxPropertyEdiorControl(resources.GetString("Editor"), emp.LongName, empID)
                          : new FaxPropertyControl(resources.GetString("Editor"), emp.LongName);
                dialog.PutControl(fpc);

                // ��������
                DateTime editDate = (DateTime) dr[Environment.FaxInData.EditedField] + LocalObject.GetTimeDiff();
                fpc = new FaxPropertyControl(resources.GetString("Edited"), editDate.ToString());
                dialog.PutControl(fpc);
                dialog.Text = resources.GetString("FaxInText");
                dialog.ResizeForm();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public static void Fax0utFillClass(PropertiesFaxDialog dialog, int faxID, DataRow dr)
        {
            try
            {
                var resources = new ResourceManager(typeof (FaxFillClass));
                if (dr == null)
                    return;

                // ������� ���������
                int imageID = 0;
                object obj = dr[Environment.FaxOutData.DocImageIDField];
                if (obj != null && !obj.Equals(DBNull.Value))
                    imageID = (int) obj;

                FaxPropertyControl fpc;

                if (imageID > 0)
                {
                    fpc = new FaxPropertiButtonControl("", resources.GetString("FaxSaved"), 2);
                    ((FaxPropertiButtonControl) fpc).FaxID = faxID;
                }
                else
                    fpc = new FaxPropertyControl("", resources.GetString("FaxNotSave"));
                dialog.PutControl(fpc);
                // ��������
                fpc = new FaxPropertyControl(resources.GetString("Description"),
                                             (string) dr[Environment.FaxOutData.DescriptionField]);
                dialog.PutControl(fpc);

                // ���������
                var date = (DateTime) dr[Environment.FaxOutData.DateField];
                fpc = new FaxPropertyControl(resources.GetString("FaxOut"),
                                             ObjectToString.Convert(date.ToLocalTime()));
                dialog.PutControl(fpc);

                // �����������
                fpc = new FaxPropertyControl(resources.GetString("Sender"),
                                             (string) dr[Environment.FaxOutData.SenderField]);
                dialog.PutControl(fpc);

                // ����������
                if (Equals(dr[Environment.FaxOutData.RecipField], dr[Environment.FaxOutData.RecvAddressField]))
                    fpc = new FaxPropertiButtonControl(resources.GetString("Recip"),
                                                       (string) dr[Environment.FaxOutData.RecipField], 1);
                else
                    fpc = new FaxPropertyControl(resources.GetString("Recip"),
                                                 (string) dr[Environment.FaxOutData.RecipField]);
                dialog.PutControl(fpc);

                // ������� ����������
                fpc = new FaxPropertyControl(resources.GetString("RecvAddress"),
                                             (string) dr[Environment.FaxOutData.RecvAddressField]);
                dialog.PutControl(fpc);

                // CSID
                fpc = new FaxPropertyControl("CSID:", (string) dr[Environment.FaxOutData.CSIDField]);
                dialog.PutControl(fpc);

                // Modem
                fpc = new FaxPropertyControl(resources.GetString("ModemID"),
                                             dr[Environment.FaxOutData.ModemIDField].ToString());
                dialog.PutControl(fpc);

                // �������� ��������
                string speed = dr[Environment.FaxOutData.SpeedField].ToString();
                fpc = new FaxPropertyControl(resources.GetString("Speed"),
                                             speed + ((speed.Length > 0) ? " " + resources.GetString("baud") : ""));
                dialog.PutControl(fpc);

                // ����� ��������
                string duration = dr[Environment.FaxOutData.DurationField].ToString();
                fpc = new FaxPropertyControl(resources.GetString("Time"),
                                             duration +
                                             ((duration.Length > 0) ? " " + resources.GetString("Second") : ""));
                dialog.PutControl(fpc);

                // ���������� �������
                fpc = new FaxPropertyControl(resources.GetString("PageSentCount"),
                                             dr[Environment.FaxOutData.PageSentCountField].ToString());
                dialog.PutControl(fpc);

                // �������
                var empID = (int) dr[Environment.FaxInData.EditorField];
                var emp = new Employee(empID, Environment.EmpData);
                fpc = empID > 0
                          ? new FaxPropertyEdiorControl(resources.GetString("Editor"), emp.LongName, empID)
                          : new FaxPropertyControl(resources.GetString("Editor"), emp.LongName);
                dialog.PutControl(fpc);

                // ��������
                DateTime editDate = (DateTime) dr[Environment.FaxInData.EditedField] + LocalObject.GetTimeDiff();
                fpc = new FaxPropertyControl(resources.GetString("Edited"), editDate.ToString());
                dialog.PutControl(fpc);
                dialog.Text = resources.GetString("FaxOutText");
                dialog.ResizeForm();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }
    }
}