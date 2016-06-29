using Antlr3.ST;
using ExportWizard.DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportWizard.DAL
{
    public class ExportSerializer
    {

        public string Serialize(ExportModel export)
        {
            string serialized = "";
            int exportSequence = 1;

            StringTemplate st = new StringTemplate(procedureShellTemplate);

            st.SetAttribute("resort", WithQuotes(export.Resort));
            st.SetAttribute("fileType", WithQuotes(export.MainExport.header.FileType));
            st.SetAttribute("fileDescription", WithQuotes(export.MainExport.header.FileDescription));

            string content = "";
            Boolean vIsComponent = false;
            if (export.SubExports != null && export.SubExports.Count > 0)
                vIsComponent = true;

            PrintQuery(export.MainExport);

            string header = Serialize(export.MainExport.header, false, vIsComponent, exportSequence++);
            string column = Serialize(export.MainExport.header.FileType, export.MainExport.columns);

            content += header + "\n" + column;

            if (export.SubExports != null)
            {
                foreach (var exportRecord in export.SubExports)
                {
                    PrintQuery(exportRecord);

                    header = Serialize(exportRecord.header, true, false, exportSequence++);
                    column = Serialize(exportRecord.header.FileType, exportRecord.columns, true);

                    content += header + "\n" + column;
                }
            }


            st.SetAttribute("content", content);

            serialized = st.ToString();

            return serialized;
        }

        public string Serialize(HeaderModel header, bool isSubHeader = false, bool isComponent = false, int exportSequence = 1)
        {
            string serialized = "";
            try
            {

                StringTemplate st = new StringTemplate(newHeaderTemplate);
                if (isSubHeader)
                {
                    st = new StringTemplate(newSubHeaderTemplate);
                }

                st.SetAttribute("file_type", WithQuotes(header.FileType));
                st.SetAttribute("file_type_desc", WithQuotes(header.FileDescription));
                st.SetAttribute("exp_file_id", "rec.exp_hdr_id");
                st.SetAttribute("resort", "rec.resort");
                st.SetAttribute("source_view_code", WithQuotes(header.SourceViewCode));
                st.SetAttribute("file_name", WithQuotes(header.FileName));
                st.SetAttribute("where_clause", WithQuotes(header.WhereClause));
                st.SetAttribute("export_sequence", exportSequence);

                if (isComponent)
                    st.SetAttribute("is_component", WithQuotes("Y"));
                else
                    st.SetAttribute("is_component", WithQuotes("N"));

                if (isSubHeader)
                    st.SetAttribute("parent_id", "rec.exp_hdr_id");
                else
                    st.SetAttribute("parent_id", "null");

                serialized = st.ToString();

            }
            catch (Exception ex)
            {

                throw new ArgumentException("Could Not Serialize Header");
            }

            return serialized;
        }

        public string Serialize(String fileType, List<ColumnModel> columns, bool isSubColumn = false)
        {
            String serialized = "";
            int columnId = 1;

            // Export Type column
            serialized += Serialize(new ColumnModel()
            {
                ColName = "Type",
                DatabaseYn = 'N'
            }, columnId++, true, isSubColumn);

            serialized += Serialize(new ColumnModel()
            {
                ColName = fileType,
                DatabaseYn = 'N',
                ColType = "FORMULA",

            }, columnId++, false, isSubColumn);


            // All other columns
            foreach (var column in columns)
            {
                // Header
                serialized += Serialize(column, columnId++, true, isSubColumn);

                //Data
                serialized += Serialize(column, columnId++, false, isSubColumn); // +"\n----------------------------------------------------------------------------------------------\n";
            }


            return serialized;
        }

        public string Serialize(ColumnModel column, int columnId, bool isHeader = false, bool isSubColumn = false)
        {
            string serialized = "";
            try
            {
                StringTemplate st = new StringTemplate(newColumnTemplate);
                if (isSubColumn)
                {
                    st = new StringTemplate(newSubColumnTemplate);
                }
                //insert_column(rec.exp_hdr_id, $exp_file_dtl_id$,  $col_name$ , $col_type$, $order_by$ );
                st.SetAttribute("exp_file_id", "rec.exp_hdr_id");
                st.SetAttribute("exp_file_dtl_id", columnId);
                st.SetAttribute("col_name", WithQuotes(column.ColName));
                st.SetAttribute("order_by", WithQuotes(column.OrderBy));
                // Type & Default Length in paranthesis
                // NUMBER(13) , VARCHAR2(4000) , DATE(11)


                if (isHeader)
                {
                    st.SetAttribute("col_type", WithQuotes("FORMULA"));
                    st.SetAttribute("gen_type", WithQuotes("H"));
                    st.SetAttribute("database_yn", WithQuotes("N"));
                    st.SetAttribute("formula", WithQuotes("'" + column.ColName.ToUpper() + "'"));
                }
                else
                {
                    st.SetAttribute("gen_type", WithQuotes("D"));

                    if (!column.DatabaseYn.Equals('N'))
                    {
                        st.SetAttribute("col_type", WithQuotes(column.ColType));
                        st.SetAttribute("database_yn", WithQuotes("Y"));
                        st.SetAttribute("formula", WithQuotes(""));
                    }
                    else
                    {
                        st.SetAttribute("col_type", WithQuotes("FORMULA"));
                        st.SetAttribute("database_yn", WithQuotes("N"));
                        st.SetAttribute("formula", WithQuotes("'" + column.ColName.ToUpper() + "'"));
                    }
                }


                serialized = st.ToString();
            }
            catch (Exception ex)
            {

                throw new ArgumentException("Could Not Serialize Column " + column.ColName);
            }

            return serialized;
        }

        public string Serialize(DeliveryModel delivery)
        {
            string serialized = "";
            try
            {
                StringTemplate st = new StringTemplate(newColumnTemplate);

                st.SetAttribute("exp_file_id", "rec.exp_hdr_id");
                st.SetAttribute("comm_type", delivery.CommType);
                st.SetAttribute("host_url", WithQuotes(delivery.HostUrl));
                st.SetAttribute("user_id", WithQuotes(delivery.UserId));
                st.SetAttribute("password", WithQuotes(delivery.Password));
                st.SetAttribute("safe_create_yn", WithQuotes("N"));
                st.SetAttribute("ftp_passive_yn", WithQuotes("N"));
                st.SetAttribute("ascii_transfer_yn", WithQuotes("N"));
                st.SetAttribute("compress_yn", WithQuotes("N"));
                st.SetAttribute("retry_count", 10);
                st.SetAttribute("retry_interval_sec", 600);
                st.SetAttribute("insert_user", -1);
                st.SetAttribute("insert_date", "sysdate");
                st.SetAttribute("update_user", -1);
                st.SetAttribute("update_date", "sysdate");

                serialized = st.ToString();
            }
            catch (Exception ex)
            {

                throw new ArgumentException("Could Not Serialize DELIVERY ");
            }

            return serialized;
        }

        private string WithQuotes(String input)
        {
            if (input == null || input.Equals(""))
            {
                return "NULL";
            }
            else
            {
                input = input.Replace("'", "''");
                return "'" + input + "'";
            }

        }

        private string WithQuotes(int input)
        {

            try
            {
                return "'" + Convert.ToString(input) + "'";
            }
            catch (Exception ex)
            {

                return "'CANNOT CONVERT PARAMETER'";
            }

        }

        // Log the export query, including the Where Clause.
        private void PrintQuery(ExportRecord exportRecord)
        {
            if (exportRecord.columns == null)
                throw new ArgumentException("No Columns defined for " + exportRecord.header.FileType);

            string select = "Select ";
            foreach(var column in exportRecord.columns)
            {
                select += column.ColName + ", ";
            }

            select = select.Remove(select.Length -2, 2);

            select += " from " + exportRecord.header.SourceViewCode;

            //TODO: Add Where
            Logger.Debug(select);
        }

        #region Templates
        private string procedureShellTemplate =
@"
Declare

  vResort   varchar2(20) := $resort$; 
  vFileType varchar2(200) := $fileType$;
  vSecondaryId Number :=0;

  procedure enable_view(sourceView Varchar2) is
  
    existingView number;
  begin
    select count(*) into existingView from exp_source_view where upper(source_view) = upper(sourceView);
    if existingView = 0 then
    
      insert into exp_source_view (SOURCE_VIEW, DESCRIPTION, INSERT_USER, INSERT_DATE, UPDATE_USER, UPDATE_DATE)
         values (upper(sourceView),'Export View '||sourceView,-1,sysdate,-1,sysdate);
    end if;
  end;

  procedure insert_header (fileType Varchar2, fileDescription Varchar2, expFileId Number, resort Varchar2, sourceViewCode Varchar2, filename Varchar2, whereClause Varchar2, componentExportYN Varchar2 := null, expParentId Number := null, exportSequence Number := null ) is
  begin
    INSERT INTO EXP_FILE_HDR 
          (
              exp_file_id,
              file_type,
              file_group_id,
              resort,
              file_type_desc,
              source_view_code,
              file_name,
              file_extension,
              col_seperator,
              where_clause,
              run_in_na_yn,
              insert_user,
              insert_date,
              update_user,
              update_date,
              inactive_yn,
              compressdata_yn,
              batch_seq,
              add_newline_yn,
              na_frequency,
              ftp_upload_yn,
              oxi_interface_id,
              na_when_to_export,
              http_upload_yn,
              component_export_yn,
              append_to_file_yn,
              xml_yn,
              sftp_yn,
              always_hdrfooter_yn,
              parent_id,
              export_sequence
          ) VALUES
          ( 
              expFileId,
              fileType,
              'MISC',
              resort,
              fileDescription,
              sourceViewCode ,
              filename,
              '''csv''',
              'TAB',
              whereClause,
              'Y',
              -1,
              sysdate,
              -1,
              sysdate,
              'N',
              'Y',
              3,
              'Y',
              NULL,
              'N',
              NULL,
              1,
              'N',
              componentExportYN,
              'N',
              'N',
              'N',
              'Y',
              expParentId,
              exportSequence
          );
  end;


  procedure insert_column ( expFileId Number, colId Number,  colName Varchar2 , colType Varchar2, formula Varchar2, databaseYN Varchar2, orderBy Number, genType Varchar2 := 'D' ) is
  begin
    insert into exp_file_dtl
          (
              exp_file_id,
              exp_file_dtl_id,
              col_name,
              col_length,
              col_alignment, 
              order_by,
              col_type,
              formula,
              ignore_length_yn,
              database_yn,
              insert_user,
              insert_date,
              update_user,
              update_date,
              gen_data_type
          ) values
          (
              expFileId,
              colId,
              colName,
              0,
              'L', 
              orderBy,
              colType,
              formula,
              'Y',
              databaseYN,
              -1,
              sysdate,
              -1,
              sysdate,
              genType		 
          );
   end;

BEGIN
        FOR rec IN
        (SELECT b.Resort,
                ( exp_file_id_seqno.nextval
                )
                exp_hdr_id
        FROM    Resort_view b
        WHERE   NOT EXISTS
                (SELECT c.resort
                FROM    exp_file_hdr c
                WHERE   file_type = vFileType
                AND     c.resort  = b.resort
                )
                AND 	( b.resort = vResort or vResort = '*' ))
        LOOP BEGIN 
          dbms_output.put_line('Creating Exports into '|| vResort||' exp_hdr_id:'||rec.exp_hdr_id);

          $content$
        END; END LOOP;
END;


";
        private string newHeaderTemplate =
@"
enable_view($source_view_code$);

insert_header ($file_type$, $file_type_desc$, $exp_file_id$, $resort$, $source_view_code$, $file_name$, $where_clause$, $is_component$ ) ;";

        private string newColumnTemplate =
@"insert_column ( rec.exp_hdr_id, $exp_file_dtl_id$,  $col_name$ , $col_type$, $formula$, $database_yn$ ,$order_by$, $gen_type$ );
";

        private string newSubHeaderTemplate = @"
enable_view($source_view_code$);

select exp_file_id_seqno.nextval
            into vSecondaryId
            from dual;

insert_header ($file_type$, $file_type_desc$, vSecondaryId, $resort$, $source_view_code$, $file_name$, $where_clause$, 'N', $parent_id$, $export_sequence$ ) ;";

        private string newSubColumnTemplate = @"
insert_column ( vSecondaryId, $exp_file_dtl_id$,  $col_name$ , $col_type$, $formula$, $database_yn$ ,$order_by$, $gen_type$ );";

        private string deliveryTemplate =
@"
INSERT INTO exp_file_delivery
(
    exp_file_id,
    comm_type,
    host_url,
    user_id,
    password,
    safe_create_yn,
    ftp_passive_yn,
    ascii_transfer_yn,
    compress_yn,
    soap_yn,
    retry_count,
    retry_interval_sec,
    insert_date,
    insert_user,
    update_date ,
    update_user
)
VALUES
(
    $exp_file_id$,
    $comm_type$,
    $host_url$,
    $user_id$,
    $password$,
    $safe_create_yn$,
    $ftp_passive_yn$,
    $ascii_transfer_yn$,
    $compress_yn$,
    $soap_yn$,
    $retry_count$,
    $retry_interval_sec$,
    $insert_date$,
    $insert_user$,
    $update_date$,
    $update_user$
);
";
        #endregion

    }
}
