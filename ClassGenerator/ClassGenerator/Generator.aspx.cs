using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace ClassGenerator
{
    public partial class Generator : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtField.Text += "pkField,pk;" + Environment.NewLine;
                txtField.Text += "intField,int;" + Environment.NewLine;
                txtField.Text += "doubleField,double;" + Environment.NewLine;
                txtField.Text += "stringField,str;" + Environment.NewLine;
                txtField.Text += "booleanField,bool;" + Environment.NewLine;
                txtField.Text += "datetimeField,dt";
            }
        }

        protected void btnGenerator_Click(object sender, EventArgs e)
        {
            //strip all [ ]
            txtField.Text = txtField.Text.Replace("[", "");
            txtField.Text = txtField.Text.Replace("]", "");
            txtClassCode.Text = GenerateClass(txtClassName.Text, txtTableName.Text, txtField.Text, true);
            txtClassCodeInlineSQL.Text = GenerateClass(txtClassName.Text, txtTableName.Text, txtField.Text, false);

            txtStoredProcedure.Text = GenerateStoredProcedure(txtClassName.Text, txtTableName.Text, txtField.Text);
        }

        private string GenerateStoredProcedure(string className, string tableName, string fieldText)
        {
            var generatedString = "";

            generatedString += GenerateStoredProcedureCreate(className, tableName, fieldText);
            generatedString += Environment.NewLine + Environment.NewLine;
            generatedString += GenerateStoredProcedureUpdate(className, tableName, fieldText);
            generatedString += Environment.NewLine + Environment.NewLine;
            generatedString += GenerateStoredProcedureDelete(className, tableName, fieldText);
            generatedString += Environment.NewLine + Environment.NewLine;
            generatedString += GenerateStoredProcedureRead(className, tableName, fieldText);

            return generatedString;
        }

        private string GenerateClass(string className, string tableName, string fieldText, bool useStoredProcedure)
        {
            var generatedString = "";

            generatedString += "public class " + className + Environment.NewLine;
            generatedString += "{" + Environment.NewLine;

            //loop though the multiline
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "pk":
                        generatedString += "\tpublic int " + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " { get; set; }" +
                                           Environment.NewLine + Environment.NewLine;
                        break;
                    case "int":
                        generatedString += "\tpublic int " + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " { get; set; } = -1;" +
                                           Environment.NewLine + Environment.NewLine;
                        break;
                    case "str":
                        generatedString += "\tpublic string " + fieldWord[0].Replace(Environment.NewLine, string.Empty) +
                                           " { get; set; } = \"\";" +
                                           Environment.NewLine + Environment.NewLine;
                        break;
                    case "dt":
                        generatedString += "\tpublic DateTime " + fieldWord[0].Replace(Environment.NewLine, string.Empty) +
                                           " { get; set; } = DateTime.Now;" +
                                           Environment.NewLine + Environment.NewLine;
                        break;
                    case "bool":
                        generatedString += "\tpublic Boolean " + fieldWord[0].Replace(Environment.NewLine, string.Empty) +
                                           " { get; set; } = false;" +
                                           Environment.NewLine + Environment.NewLine;
                        break;
                    case "double":
                        generatedString += "\tpublic double " + fieldWord[0].Replace(Environment.NewLine, string.Empty) +
                                           " { get; set; } = 0;" +
                                           Environment.NewLine + Environment.NewLine;
                        break;
                }
            }

            generatedString += GenerateClassStoredProcedureCreate(className, tableName, fieldText, useStoredProcedure) + Environment.NewLine + Environment.NewLine;

            generatedString += GenerateClassStoredProcedureUpdate(className, tableName, fieldText, useStoredProcedure) + Environment.NewLine + Environment.NewLine;

            generatedString += GenerateClassStoredProcedureDelete(className, tableName, fieldText, useStoredProcedure) + Environment.NewLine + Environment.NewLine;

            generatedString += GenerateClassStoredProcedureRead(className, tableName, fieldText, useStoredProcedure);

            generatedString += Environment.NewLine + "}";

            return generatedString;
        }

        private string GenerateClassStoredProcedureCreate(string className, string tableName, string fieldText, bool useStoredProcedure)
        {
            string generatedString = "";

            generatedString += "\tpublic Boolean CreateRecord() " + Environment.NewLine + "\t{" + Environment.NewLine;
            generatedString += "\t\tBoolean result = false;" + Environment.NewLine + Environment.NewLine;
            generatedString += "\t\tSqlConnection conn = new SqlConnection(connectionstring());" + Environment.NewLine + Environment.NewLine;
            generatedString += "\t\ttry" + Environment.NewLine + "\t\t{" + Environment.NewLine;
            generatedString += "\t\t\tconn.Open();" + Environment.NewLine;
            if (useStoredProcedure)
            {
                generatedString += "\t\t\tstring strsql = \"" + txtSPPrefix.Text + "_NewRecord\";" + Environment.NewLine;
                generatedString +=
                    "\t\t\tSqlCommand cmd = new SqlCommand(strsql, conn) {CommandType = CommandType.StoredProcedure};" +
                    Environment.NewLine;
            }
            else
            {
                string inlineSQL = "";

                inlineSQL += "INSERT INTO " + tableName + " (";
                foreach (var field in fieldText.Split(';'))
                {
                    var fieldWord = field.Split(',');
                    //fieldWord[0] = fieldName
                    //fieldWord[1] = fieldType

                    switch (fieldWord[1])
                    {
                        case "int":
                        case "str":
                        case "dt":
                        case "bool":
                        case "double":
                            inlineSQL += fieldWord[0].Replace(Environment.NewLine, string.Empty) + ", ";
                            break;
                        case "pk":
                            break;
                    }
                }
                inlineSQL = inlineSQL.TrimEnd(',', ' ');
                inlineSQL += ") \" +" + Environment.NewLine;
                inlineSQL += "\t\t\t\t\t\t\" VALUES (";
                foreach (var field in fieldText.Split(';'))
                {
                    var fieldWord = field.Split(',');
                    //fieldWord[0] = fieldName
                    //fieldWord[1] = fieldType

                    switch (fieldWord[1])
                    {
                        case "int":
                        case "str":
                        case "dt":
                        case "bool":
                        case "double":
                            inlineSQL += "@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + ", ";
                            break;
                        case "pk":
                            break;
                    }
                }
                inlineSQL = inlineSQL.TrimEnd(',', ' ');
                inlineSQL += ")";

                generatedString += "\t\t\tstring strsql = \"" + inlineSQL + "\";" + Environment.NewLine;
                generatedString +=
                    "\t\t\tSqlCommand cmd = new SqlCommand(strsql, conn);" +
                    Environment.NewLine;
            }


            //loop though the multiline
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');

                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType
                if (fieldWord[1] != "pk")
                {
                    generatedString += "\t\t\tcmd.Parameters.AddWithValue(\"@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "\", " + fieldWord[0].Replace(Environment.NewLine, string.Empty) + ");" + Environment.NewLine;
                }
            }

            generatedString += Environment.NewLine + "\t\t\tif (cmd.ExecuteNonQuery() > 0) { result = true; }" +
                               Environment.NewLine;
            generatedString += "\t\t} catch (Exception ex) {" + Environment.NewLine;
            generatedString += "\t\t\tSendExcepToDB(ex);" + Environment.NewLine;
            generatedString += "\t\t} finally {" + Environment.NewLine;
            generatedString += "\t\t\tconn.Close();" + Environment.NewLine + "\t\t}";
            generatedString += Environment.NewLine + Environment.NewLine + "\t\treturn result;";
            generatedString += Environment.NewLine + "\t}";

            return generatedString;
        }

        private string GenerateClassStoredProcedureUpdate(string className, string tableName, string fieldText, bool useStoredProcedure)
        {
            string generatedString = "";

            generatedString += "\tpublic Boolean UpdateRecord() " + Environment.NewLine + "\t{" + Environment.NewLine;
            generatedString += "\t\tBoolean result = false;" + Environment.NewLine + Environment.NewLine;
            generatedString += "\t\tSqlConnection conn = new SqlConnection(connectionstring());" + Environment.NewLine + Environment.NewLine;
            generatedString += "\t\ttry" + Environment.NewLine + "\t\t{" + Environment.NewLine;
            generatedString += "\t\t\tconn.Open();" + Environment.NewLine;
            if (useStoredProcedure)
            {
                generatedString += "\t\t\tstring strsql = \"" + txtSPPrefix.Text + "_UpdateRecord\";" + Environment.NewLine;
                generatedString += "\t\t\tSqlCommand cmd = new SqlCommand(strsql, conn) {CommandType = CommandType.StoredProcedure};" + Environment.NewLine;
            }
            else
            {
                string inlineSQL = "";

                inlineSQL += "UPDATE " + tableName + " SET ";
                foreach (var field in fieldText.Split(';'))
                {
                    var fieldWord = field.Split(',');
                    //fieldWord[0] = fieldName
                    //fieldWord[1] = fieldType

                    switch (fieldWord[1])
                    {
                        case "int":
                        case "str":
                        case "dt":
                        case "bool":
                        case "double":
                            inlineSQL += fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = @" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + ", ";
                            break;
                        case "pk":
                            break;
                    }
                }
                inlineSQL = inlineSQL.TrimEnd(',',' ');
                inlineSQL += "\" +" + Environment.NewLine;
                inlineSQL += "\t\t\t\t\t\t\" WHERE ";
                foreach (var field in fieldText.Split(';'))
                {
                    var fieldWord = field.Split(',');
                    //fieldWord[0] = fieldName
                    //fieldWord[1] = fieldType

                    switch (fieldWord[1])
                    {
                        case "int":
                        case "str":
                        case "dt":
                        case "bool":
                        case "double":
                            break;
                        case "pk":
                            inlineSQL += fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = @" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "" + Environment.NewLine;
                            break;
                    }
                }
                inlineSQL = inlineSQL.TrimEnd('\r', '\n');
                generatedString += "\t\t\tstring strsql = \"" + inlineSQL + "\";" + Environment.NewLine;
                generatedString += "\t\t\tSqlCommand cmd = new SqlCommand(strsql, conn);" + Environment.NewLine;
            }

            //loop though the multiline
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');

                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType
                generatedString += "\t\t\tcmd.Parameters.AddWithValue(\"@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "\", " + fieldWord[0].Replace(Environment.NewLine, string.Empty) + ");" + Environment.NewLine;
            }

            generatedString += Environment.NewLine + "\t\t\tif (cmd.ExecuteNonQuery() > 0) { result = true; }" +
                               Environment.NewLine;
            generatedString += "\t\t} catch (Exception ex) {" + Environment.NewLine;
            generatedString += "\t\t\tSendExcepToDB(ex);" + Environment.NewLine;
            generatedString += "\t\t} finally {" + Environment.NewLine;
            generatedString += "\t\t\tconn.Close();" + Environment.NewLine + "\t\t}";
            generatedString += Environment.NewLine + Environment.NewLine + "\t\treturn result;";
            generatedString += Environment.NewLine + "\t}";

            return generatedString;
        }

        private string GenerateClassStoredProcedureDelete(string className, string tableName, string fieldText, bool useStoredProcedure)
        {
            string generatedString = "";

            generatedString += "\tpublic Boolean DeleteRecord() " + Environment.NewLine + "\t{" + Environment.NewLine;
            generatedString += "\t\tBoolean result = false;" + Environment.NewLine + Environment.NewLine;
            generatedString += "\t\tSqlConnection conn = new SqlConnection(connectionstring());" + Environment.NewLine + Environment.NewLine;
            generatedString += "\t\ttry" + Environment.NewLine + "\t\t{" + Environment.NewLine;
            generatedString += "\t\t\tconn.Open();" + Environment.NewLine;
            if (useStoredProcedure)
            {
                generatedString += "\t\t\tstring strsql = \"" + txtSPPrefix.Text + "_DeleteRecord\";" + Environment.NewLine;
                generatedString +=
                    "\t\t\tSqlCommand cmd = new SqlCommand(strsql, conn) {CommandType = CommandType.StoredProcedure};" +
                    Environment.NewLine;
            }
            else
            {
                string inlineSQL = "";

                inlineSQL += "DELETE FROM " + tableName + "";
                inlineSQL += "\" +" + Environment.NewLine;
                inlineSQL += "\t\t\t\t\t\t\" WHERE ";
                foreach (var field in fieldText.Split(';'))
                {
                    var fieldWord = field.Split(',');
                    //fieldWord[0] = fieldName
                    //fieldWord[1] = fieldType

                    switch (fieldWord[1])
                    {
                        case "int":
                        case "str":
                        case "dt":
                        case "bool":
                        case "double":
                            break;
                        case "pk":
                            inlineSQL += fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = @" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "" + Environment.NewLine;
                            break;
                    }
                }
                inlineSQL = inlineSQL.TrimEnd('\r', '\n');
                generatedString += "\t\t\tstring strsql = \"" + inlineSQL + "\";" + Environment.NewLine;
                generatedString += "\t\t\tSqlCommand cmd = new SqlCommand(strsql, conn);" + Environment.NewLine;
            }

            //loop though the multiline
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');

                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType
                if (fieldWord[1] == "pk")
                {
                    generatedString += "\t\t\tcmd.Parameters.AddWithValue(\"@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "\", " + fieldWord[0].Replace(Environment.NewLine, string.Empty) + ");" + Environment.NewLine;
                }
            }

            generatedString += Environment.NewLine + "\t\t\tif (cmd.ExecuteNonQuery() > 0) { result = true; }" +
                               Environment.NewLine;
            generatedString += "\t\t} catch (Exception ex) {" + Environment.NewLine;
            generatedString += "\t\t\tSendExcepToDB(ex);" + Environment.NewLine;
            generatedString += "\t\t} finally {" + Environment.NewLine;
            generatedString += "\t\t\tconn.Close();" + Environment.NewLine + "\t\t}";
            generatedString += Environment.NewLine + Environment.NewLine + "\t\treturn result;";
            generatedString += Environment.NewLine + "\t}";

            return generatedString;
        }

        private string GenerateClassStoredProcedureRead(string className, string tableName, string fieldText, bool useStoredProcedure)
        {
            string generatedString = "";

            generatedString += "\tpublic Boolean ReadRecord() " + Environment.NewLine + "\t{" + Environment.NewLine;
            generatedString += "\t\tBoolean result = false;" + Environment.NewLine + Environment.NewLine;
            generatedString += "\t\tSqlConnection conn = new SqlConnection(connectionstring());" + Environment.NewLine + Environment.NewLine;
            generatedString += "\t\ttry" + Environment.NewLine + "\t\t{" + Environment.NewLine;
            generatedString += "\t\t\tconn.Open();" + Environment.NewLine;
            if (useStoredProcedure)
            {
                generatedString += "\t\t\tstring strsql = \"" + txtSPPrefix.Text + "_ReadRecord\";" + Environment.NewLine;
                generatedString +=
                    "\t\t\tSqlCommand cmd = new SqlCommand(strsql, conn) {CommandType = CommandType.StoredProcedure};" +
                    Environment.NewLine;
            }
            else
            {
                string inlineSQL = "";

                inlineSQL += "SELECT ";
                //loop though the multiline
                foreach (var field in fieldText.Split(';'))
                {
                    var fieldWord = field.Split(',');
                    //fieldWord[0] = fieldName
                    //fieldWord[1] = fieldType

                    switch (fieldWord[1])
                    {
                        case "int":
                        case "str":
                        case "dt":
                        case "bool":
                        case "double":
                        case "pk":
                            inlineSQL += fieldWord[0].Replace(Environment.NewLine, string.Empty) + ", ";
                            break;
                    }
                }
                inlineSQL = inlineSQL.TrimEnd(',', ' ');
                inlineSQL += "\" +";
                inlineSQL += Environment.NewLine + "\t\t\t\t\t\t\" FROM " + tableName + " WHERE ";
                foreach (var field in fieldText.Split(';'))
                {
                    var fieldWord = field.Split(',');
                    //fieldWord[0] = fieldName
                    //fieldWord[1] = fieldType

                    switch (fieldWord[1])
                    {
                        case "int":
                        case "str":
                        case "dt":
                        case "bool":
                        case "double":
                            break;
                        case "pk":
                            inlineSQL += fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = @" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "" + Environment.NewLine;
                            break;
                    }
                }
                inlineSQL = inlineSQL.TrimEnd('\r', '\n');
                generatedString += "\t\t\tstring strsql = \"" + inlineSQL + "\";" + Environment.NewLine;
                generatedString += "\t\t\tSqlCommand cmd = new SqlCommand(strsql, conn);" + Environment.NewLine;
            }

            //loop though the multiline
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');

                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType
                if (fieldWord[1] == "pk")
                {
                    generatedString += "\t\t\tcmd.Parameters.AddWithValue(\"@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "\", " + fieldWord[0].Replace(Environment.NewLine, string.Empty) + ");" + Environment.NewLine;
                }
            }

            generatedString += Environment.NewLine +
                               "\t\t\tSqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);" + Environment.NewLine + Environment.NewLine;

            generatedString += "\t\t\tif (dr.HasRows)" + Environment.NewLine + "\t\t\t{" + Environment.NewLine + "\t\t\t\tdr.Read();" + Environment.NewLine + "\t\t\t\tresult = true;" + Environment.NewLine + Environment.NewLine;

            //looping
            foreach (var field in txtField.Text.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "pk":
                    case "int":
                        generatedString += "\t\t\t\t" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = (int)" +
                                           "dr[\"" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "\"];" +
                                           Environment.NewLine;
                        break;
                    case "str":
                        generatedString += "\t\t\t\t" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = (string)" + "dr[\"" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "\"];" +
                                           Environment.NewLine;
                        break;
                    case "dt":
                        generatedString += "\t\t\t\t" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = (DateTime)" + "dr[\"" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "\"];" +
                                           Environment.NewLine;
                        break;
                    case "bool":
                        generatedString += "\t\t\t\t" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = (Boolean)" + "dr[\"" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "\"];" +
                                           Environment.NewLine;
                        break;
                    case "double":
                        generatedString += "\t\t\t\t" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = (double)" + "dr[\"" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "\"];" +
                                           Environment.NewLine;
                        break;
                }
            }

            generatedString += "\t\t\t}" + Environment.NewLine;

            generatedString += "\t\t} catch (Exception ex) {" + Environment.NewLine;
            generatedString += "\t\t\tSendExcepToDB(ex);" + Environment.NewLine;
            generatedString += "\t\t} finally {" + Environment.NewLine;
            generatedString += "\t\t\tconn.Close();" + Environment.NewLine + "\t\t}";
            generatedString += Environment.NewLine + Environment.NewLine + "\t\treturn result;";
            generatedString += Environment.NewLine + "\t}";

            return generatedString;
        }

        private string GenerateStoredProcedureCreate(string className, string tableName, string fieldText)
        {
            string generatedString = "";

            generatedString += "-- INSERT PROCEDURE" + Environment.NewLine;
            generatedString += "CREATE PROCEDURE " + txtSPPrefix.Text + "_NewRecord" + Environment.NewLine;

            //loop though the multiline
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                        generatedString +=  "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " int," + Environment.NewLine;
                        break;
                    case "str":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " nvarchar(MAX)," + Environment.NewLine;
                        break;
                    case "dt":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " DATETIME2(7)," + Environment.NewLine;
                        break;
                    case "bool":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " bit," + Environment.NewLine;
                        break;
                    case "double":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " decimal(19,2)," + Environment.NewLine;
                        break;
                    case "pk":
                        break;
                }
            }
            generatedString = generatedString.TrimEnd(',', '\r', '\n');

            generatedString += Environment.NewLine + "AS" + Environment.NewLine + "BEGIN" + Environment.NewLine;
            generatedString += "\t" + "SET NOCOUNT OFF;" + Environment.NewLine;
            generatedString += "\t" + "INSERT INTO "+ tableName +"(" + Environment.NewLine;
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                    case "str":
                    case "dt":
                    case "bool":
                    case "double":
                        generatedString += "\t" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "," + Environment.NewLine;
                        break;
                    case "pk":
                        break;
                }
            }
            generatedString = generatedString.TrimEnd(',', '\r', '\n');
            generatedString += ")" + Environment.NewLine;
            generatedString += "VALUES (" + Environment.NewLine;
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                    case "str":
                    case "dt":
                    case "bool":
                    case "double":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "," + Environment.NewLine;
                        break;
                    case "pk":
                        break;
                }
            }
            generatedString = generatedString.TrimEnd(',', '\r', '\n');
            generatedString += ");" + Environment.NewLine;
            generatedString += "END" + Environment.NewLine + "GO";

            return generatedString;
        }

        private string GenerateStoredProcedureUpdate(string className, string tableName, string fieldText)
        {
            string generatedString = "";

            generatedString += "-- UPDATE PROCEDURE" + Environment.NewLine;
            generatedString += "CREATE PROCEDURE " + txtSPPrefix.Text + "_UpdateRecord" + Environment.NewLine;

            //loop though the multiline
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "pk":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " int," + Environment.NewLine;
                        break;
                    case "int":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " int," + Environment.NewLine;
                        break;
                    case "str":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " nvarchar(MAX)," + Environment.NewLine;
                        break;
                    case "dt":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " DATETIME2(7)," + Environment.NewLine;
                        break;
                    case "bool":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " bit," + Environment.NewLine;
                        break;
                    case "double":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " double(19,2)," + Environment.NewLine;
                        break;
                }
            }
            generatedString = generatedString.TrimEnd(',', '\r', '\n');

            generatedString += Environment.NewLine + "AS" + Environment.NewLine + "BEGIN" + Environment.NewLine;
            generatedString += "\t" + "SET NOCOUNT OFF;" + Environment.NewLine;
            generatedString += "\t" + "UPDATE " + tableName + Environment.NewLine + "\tSET " + Environment.NewLine;
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                    case "str":
                    case "dt":
                    case "bool":
                    case "double":
                        generatedString += "\t" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = @" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "," + Environment.NewLine;
                        break;
                    case "pk":
                        break;
                }
            }
            generatedString = generatedString.TrimEnd(',', '\r', '\n');
            generatedString += Environment.NewLine + "\tWHERE ";
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                    case "str":
                    case "dt":
                    case "bool":
                    case "double":
                        break;
                    case "pk":
                        generatedString += fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = @" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "" + Environment.NewLine;
                        break;
                }
            }
            generatedString += "END" + Environment.NewLine + "GO";

            return generatedString;
        }

        private string GenerateStoredProcedureDelete(string className, string tableName, string fieldText)
        {
            string generatedString = "";

            generatedString += "-- DELETE PROCEDURE" + Environment.NewLine;
            generatedString += "CREATE PROCEDURE " + txtSPPrefix.Text + "_DeleteRecord" + Environment.NewLine;

            //loop though the multiline
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                    case "str":
                    case "dt":
                    case "bool":
                    case "double":
                        break;
                    case "pk":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " int" + Environment.NewLine;
                        break;
                }
            }
            generatedString = generatedString.TrimEnd(',', '\r', '\n');

            generatedString += Environment.NewLine + "AS" + Environment.NewLine + "BEGIN" + Environment.NewLine;
            generatedString += "\t" + "SET NOCOUNT OFF;" + Environment.NewLine;
            generatedString += "\t" + "DELETE FROM " + tableName + Environment.NewLine + "\tWHERE ";
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                    case "str":
                    case "dt":
                    case "bool":
                    case "double":
                        break;
                    case "pk":
                        generatedString += fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = @" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "" + Environment.NewLine;
                        break;
                }
            }
            generatedString += "END" + Environment.NewLine + "GO";

            return generatedString;
        }

        private string GenerateStoredProcedureRead(string className, string tableName, string fieldText)
        {
            string generatedString = "";

            generatedString += "-- SELECT PROCEDURE" + Environment.NewLine;
            generatedString += "CREATE PROCEDURE " + txtSPPrefix.Text + "_ReadRecord" + Environment.NewLine;
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                    case "str":
                    case "dt":
                    case "bool":
                    case "double":
                        break;
                    case "pk":
                        generatedString += "\t@" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + " int" + Environment.NewLine;
                        break;
                }
            }
            generatedString += "AS" + Environment.NewLine + "BEGIN" + Environment.NewLine;
            generatedString += "\tSET NOCOUNT OFF;" + Environment.NewLine;
            generatedString += "\tSELECT" + Environment.NewLine;
            //loop though the multiline
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                    case "str":
                    case "dt":
                    case "bool":
                    case "double":
                    case "pk":
                        generatedString += "\t" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "," + Environment.NewLine;
                        break;
                }
            }
            generatedString = generatedString.TrimEnd(',', '\r', '\n');
            generatedString += Environment.NewLine + "\tFROM " + txtTableName.Text + Environment.NewLine + "\tWHERE ";
            foreach (var field in fieldText.Split(';'))
            {
                var fieldWord = field.Split(',');
                //fieldWord[0] = fieldName
                //fieldWord[1] = fieldType

                switch (fieldWord[1])
                {
                    case "int":
                    case "str":
                    case "dt":
                    case "bool":
                    case "double":
                        break;
                    case "pk":
                        generatedString += fieldWord[0].Replace(Environment.NewLine, string.Empty) + " = @" + fieldWord[0].Replace(Environment.NewLine, string.Empty) + "" + Environment.NewLine;
                        break;
                }
            }
            generatedString += "END" + Environment.NewLine + "GO";

            return generatedString;
        }
    }
}