﻿#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2021 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using Transformalize.Configuration;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.PostgreSql {
   public class PostgreSqlConnectionFactory : IConnectionFactory {
      static Dictionary<string, string> _types;
      private static HashSet<string> _reserved;
      readonly Connection _c;

      public AdoProvider AdoProvider { get; } = AdoProvider.PostgreSql;
      public string Terminator { get; } = ";";

      //select * from pg_get_keywords() where catcode = 'R'
      public HashSet<string> Reserved => _reserved ??
      (_reserved =
          new HashSet<string>(StringComparer.OrdinalIgnoreCase)
          {
            "a",
            "abort",
            "abs",
            "absent",
            "absolute",
            "access",
            "according",
            "acos",
            "action",
            "ada",
            "add",
            "admin",
            "after",
            "aggregate",
            "all",
            "allocate",
            "also",
            "alter",
            "always",
            "analyse",
            "analyze",
            "and",
            "any",
            "are",
            "array",
            "array_agg",
            "array_​max_​cardinality",
            "as",
            "asc",
            "asensitive",
            "asin",
            "assertion",
            "assignment",
            "asymmetric",
            "at",
            "atan",
            "atomic",
            "attach",
            "attribute",
            "attributes",
            "authorization",
            "avg",
            "backward",
            "base64",
            "before",
            "begin",
            "begin_frame",
            "begin_partition",
            "bernoulli",
            "between",
            "bigint",
            "binary",
            "bit",
            "bit_length",
            "blob",
            "blocked",
            "bom",
            "boolean",
            "both",
            "breadth",
            "by",
            "c",
            "cache",
            "call",
            "called",
            "cardinality",
            "cascade",
            "cascaded",
            "case",
            "cast",
            "catalog",
            "catalog_name",
            "ceil",
            "ceiling",
            "chain",
            "chaining",
            "char",
            "character",
            "characteristics",
            "characters",
            "character_length",
            "character_​set_​catalog",
            "character_set_name",
            "character_set_schema",
            "char_length",
            "check",
            "checkpoint",
            "class",
            "classifier",
            "class_origin",
            "clob",
            "close",
            "cluster",
            "coalesce",
            "cobol",
            "collate",
            "collation",
            "collation_catalog",
            "collation_name",
            "collation_schema",
            "collect",
            "column",
            "columns",
            "column_name",
            "command_function",
            "command_​function_​code",
            "comment",
            "comments",
            "commit",
            "committed",
            "concurrently",
            "condition",
            "conditional",
            "condition_number",
            "configuration",
            "conflict",
            "connect",
            "connection",
            "connection_name",
            "constraint",
            "constraints",
            "constraint_catalog",
            "constraint_name",
            "constraint_schema",
            "constructor",
            "contains",
            "content",
            "continue",
            "control",
            "conversion",
            "convert",
            "copy",
            "corr",
            "corresponding",
            "cos",
            "cosh",
            "cost",
            "count",
            "covar_pop",
            "covar_samp",
            "create",
            "cross",
            "csv",
            "cube",
            "cume_dist",
            "current",
            "current_catalog",
            "current_date",
            "current_​default_​transform_​group",
            "current_path",
            "current_role",
            "current_row",
            "current_schema",
            "current_time",
            "current_timestamp",
            "current_​transform_​group_​for_​type",
            "current_user",
            "cursor",
            "cursor_name",
            "cycle",
            "data",
            "database",
            "datalink",
            "date",
            "datetime_​interval_​code",
            "datetime_​interval_​precision",
            "day",
            "db",
            "deallocate",
            "dec",
            "decfloat",
            "decimal",
            "declare",
            "default",
            "defaults",
            "deferrable",
            "deferred",
            "define",
            "defined",
            "definer",
            "degree",
            "delete",
            "delimiter",
            "delimiters",
            "dense_rank",
            "depends",
            "depth",
            "deref",
            "derived",
            "desc",
            "describe",
            "descriptor",
            "detach",
            "deterministic",
            "diagnostics",
            "dictionary",
            "disable",
            "discard",
            "disconnect",
            "dispatch",
            "distinct",
            "dlnewcopy",
            "dlpreviouscopy",
            "dlurlcomplete",
            "dlurlcompleteonly",
            "dlurlcompletewrite",
            "dlurlpath",
            "dlurlpathonly",
            "dlurlpathwrite",
            "dlurlscheme",
            "dlurlserver",
            "dlvalue",
            "do",
            "document",
            "domain",
            "double",
            "drop",
            "dynamic",
            "dynamic_function",
            "dynamic_​function_​code",
            "each",
            "element",
            "else",
            "empty",
            "enable",
            "encoding",
            "encrypted",
            "end",
            "end-exec",
            "end_frame",
            "end_partition",
            "enforced",
            "enum",
            "equals",
            "error",
            "escape",
            "event",
            "every",
            "except",
            "exception",
            "exclude",
            "excluding",
            "exclusive",
            "exec",
            "execute",
            "exists",
            "exp",
            "explain",
            "expression",
            "extension",
            "external",
            "extract",
            "false",
            "family",
            "fetch",
            "file",
            "filter",
            "final",
            "finish",
            "first",
            "first_value",
            "flag",
            "float",
            "floor",
            "following",
            "for",
            "force",
            "foreign",
            "format",
            "fortran",
            "forward",
            "found",
            "frame_row",
            "free",
            "freeze",
            "from",
            "fs",
            "fulfill",
            "full",
            "function",
            "functions",
            "fusion",
            "g",
            "general",
            "generated",
            "get",
            "global",
            "go",
            "goto",
            "grant",
            "granted",
            "greatest",
            "group",
            "grouping",
            "groups",
            "handler",
            "having",
            "header",
            "hex",
            "hierarchy",
            "hold",
            "hour",
            "id",
            "identity",
            "if",
            "ignore",
            "ilike",
            "immediate",
            "immediately",
            "immutable",
            "implementation",
            "implicit",
            "import",
            "in",
            "include",
            "including",
            "increment",
            "indent",
            "index",
            "indexes",
            "indicator",
            "inherit",
            "inherits",
            "initial",
            "initially",
            "inline",
            "inner",
            "inout",
            "input",
            "insensitive",
            "insert",
            "instance",
            "instantiable",
            "instead",
            "int",
            "integer",
            "integrity",
            "intersect",
            "intersection",
            "interval",
            "into",
            "invoker",
            "is",
            "isnull",
            "isolation",
            "join",
            "json",
            "json_array",
            "json_arrayagg",
            "json_exists",
            "json_object",
            "json_objectagg",
            "json_query",
            "json_table",
            "json_table_primitive",
            "json_value",
            "k",
            "keep",
            "key",
            "keys",
            "key_member",
            "key_type",
            "label",
            "lag",
            "language",
            "large",
            "last",
            "last_value",
            "lateral",
            "lead",
            "leading",
            "leakproof",
            "least",
            "left",
            "length",
            "level",
            "library",
            "like",
            "like_regex",
            "limit",
            "link",
            "listagg",
            "listen",
            "ln",
            "load",
            "local",
            "localtime",
            "localtimestamp",
            "location",
            "locator",
            "lock",
            "locked",
            "log",
            "log10",
            "logged",
            "lower",
            "m",
            "map",
            "mapping",
            "match",
            "matched",
            "matches",
            "match_number",
            "match_recognize",
            "materialized",
            "max",
            "maxvalue",
            "measures",
            "member",
            "merge",
            "message_length",
            "message_octet_length",
            "message_text",
            "method",
            "min",
            "minute",
            "minvalue",
            "mod",
            "mode",
            "modifies",
            "module",
            "month",
            "more",
            "move",
            "multiset",
            "mumps",
            "name",
            "names",
            "namespace",
            "national",
            "natural",
            "nchar",
            "nclob",
            "nested",
            "nesting",
            "new",
            "next",
            "nfc",
            "nfd",
            "nfkc",
            "nfkd",
            "nil",
            "no",
            "none",
            "normalize",
            "normalized",
            "not",
            "nothing",
            "notify",
            "notnull",
            "nowait",
            "nth_value",
            "ntile",
            "null",
            "nullable",
            "nullif",
            "nulls",
            "number",
            "numeric",
            "object",
            "occurrences_regex",
            "octets",
            "octet_length",
            "of",
            "off",
            "offset",
            "oids",
            "old",
            "omit",
            "on",
            "one",
            "only",
            "open",
            "operator",
            "option",
            "options",
            "or",
            "order",
            "ordering",
            "ordinality",
            "others",
            "out",
            "outer",
            "output",
            "over",
            "overflow",
            "overlaps",
            "overlay",
            "overriding",
            "owned",
            "owner",
            "p",
            "pad",
            "parallel",
            "parameter",
            "parameter_mode",
            "parameter_name",
            "parameter_​ordinal_​position",
            "parameter_​specific_​catalog",
            "parameter_​specific_​name",
            "parameter_​specific_​schema",
            "parser",
            "partial",
            "partition",
            "pascal",
            "pass",
            "passing",
            "passthrough",
            "password",
            "past",
            "path",
            "pattern",
            "per",
            "percent",
            "percentile_cont",
            "percentile_disc",
            "percent_rank",
            "period",
            "permission",
            "permute",
            "placing",
            "plan",
            "plans",
            "pli",
            "policy",
            "portion",
            "position",
            "position_regex",
            "power",
            "precedes",
            "preceding",
            "precision",
            "prepare",
            "prepared",
            "preserve",
            "primary",
            "prior",
            "private",
            "privileges",
            "procedural",
            "procedure",
            "procedures",
            "program",
            "prune",
            "ptf",
            "public",
            "publication",
            "quote",
            "quotes",
            "range",
            "rank",
            "read",
            "reads",
            "real",
            "reassign",
            "recheck",
            "recovery",
            "recursive",
            "ref",
            "references",
            "referencing",
            "refresh",
            "regr_avgx",
            "regr_avgy",
            "regr_count",
            "regr_intercept",
            "regr_r2",
            "regr_slope",
            "regr_sxx",
            "regr_sxy",
            "regr_syy",
            "reindex",
            "relative",
            "release",
            "rename",
            "repeatable",
            "replace",
            "replica",
            "requiring",
            "reset",
            "respect",
            "restart",
            "restore",
            "restrict",
            "result",
            "return",
            "returned_cardinality",
            "returned_length",
            "returned_​octet_​length",
            "returned_sqlstate",
            "returning",
            "returns",
            "revoke",
            "right",
            "role",
            "rollback",
            "rollup",
            "routine",
            "routines",
            "routine_catalog",
            "routine_name",
            "routine_schema",
            "row",
            "rows",
            "row_count",
            "row_number",
            "rule",
            "running",
            "savepoint",
            "scalar",
            "scale",
            "schema",
            "schemas",
            "schema_name",
            "scope",
            "scope_catalog",
            "scope_name",
            "scope_schema",
            "scroll",
            "search",
            "second",
            "section",
            "security",
            "seek",
            "select",
            "selective",
            "self",
            "sensitive",
            "sequence",
            "sequences",
            "serializable",
            "server",
            "server_name",
            "session",
            "session_user",
            "set",
            "setof",
            "sets",
            "share",
            "show",
            "similar",
            "simple",
            "sin",
            "sinh",
            "size",
            "skip",
            "smallint",
            "snapshot",
            "some",
            "source",
            "space",
            "specific",
            "specifictype",
            "specific_name",
            "sql",
            "sqlcode",
            "sqlerror",
            "sqlexception",
            "sqlstate",
            "sqlwarning",
            "sqrt",
            "stable",
            "standalone",
            "start",
            "state",
            "statement",
            "static",
            "statistics",
            "stddev_pop",
            "stddev_samp",
            "stdin",
            "stdout",
            "storage",
            "stored",
            "strict",
            "string",
            "strip",
            "structure",
            "style",
            "subclass_origin",
            "submultiset",
            "subscription",
            "subset",
            "substring",
            "substring_regex",
            "succeeds",
            "sum",
            "support",
            "symmetric",
            "sysid",
            "system",
            "system_time",
            "system_user",
            "t",
            "table",
            "tables",
            "tablesample",
            "tablespace",
            "table_name",
            "tan",
            "tanh",
            "temp",
            "template",
            "temporary",
            "text",
            "then",
            "through",
            "ties",
            "time",
            "timestamp",
            "timezone_hour",
            "timezone_minute",
            "to",
            "token",
            "top_level_count",
            "trailing",
            "transaction",
            "transactions_​committed",
            "transactions_​rolled_​back",
            "transaction_active",
            "transform",
            "transforms",
            "translate",
            "translate_regex",
            "translation",
            "treat",
            "trigger",
            "trigger_catalog",
            "trigger_name",
            "trigger_schema",
            "trim",
            "trim_array",
            "true",
            "truncate",
            "trusted",
            "type",
            "types",
            "uescape",
            "unbounded",
            "uncommitted",
            "unconditional",
            "under",
            "unencrypted",
            "union",
            "unique",
            "unknown",
            "unlink",
            "unlisten",
            "unlogged",
            "unmatched",
            "unnamed",
            "unnest",
            "until",
            "untyped",
            "update",
            "upper",
            "uri",
            "usage",
            "user",
            "user_​defined_​type_​catalog",
            "user_​defined_​type_​code",
            "user_​defined_​type_​name",
            "user_​defined_​type_​schema",
            "using",
            "utf16",
            "utf32",
            "utf8",
            "vacuum",
            "valid",
            "validate",
            "validator",
            "value",
            "values",
            "value_of",
            "varbinary",
            "varchar",
            "variadic",
            "varying",
            "var_pop",
            "var_samp",
            "verbose",
            "version",
            "versioning",
            "view",
            "views",
            "volatile",
            "when",
            "whenever",
            "where",
            "whitespace",
            "width_bucket",
            "window",
            "with",
            "within",
            "without",
            "work",
            "wrapper",
            "write",
            "xml",
            "xmlagg",
            "xmlattributes",
            "xmlbinary",
            "xmlcast",
            "xmlcomment",
            "xmlconcat",
            "xmldeclaration",
            "xmldocument",
            "xmlelement",
            "xmlexists",
            "xmlforest",
            "xmliterate",
            "xmlnamespaces",
            "xmlparse",
            "xmlpi",
            "xmlquery",
            "xmlroot",
            "xmlschema",
            "xmlserialize",
            "xmltable",
            "xmltext",
            "xmlvalidate",
            "year",
            "yes",
            "zone",
          });

      public Dictionary<string, string> Types => _types ?? (_types = new Dictionary<string, string> {
            {"int64", "BIGINT"},
            {"int", "INTEGER"},
            {"real", "REAL" },
            {"long", "BIGINT"},
            {"boolean", "BOOLEAN"},
            {"bool", "BOOLEAN"},
            {"string", "VARCHAR"},
            {"date", "DATE" },
            {"datetime", "TIMESTAMP"},
            {"decimal", "NUMERIC"},
            {"double", "DOUBLE PRECISION"},
            {"float", "DOUBLE PRECISION" },
            {"int32", "INTEGER"},
            {"char", "VARCHAR"},
            {"single", "REAL"},
            {"int16", "SMALLINT"},
            {"short","SMALLINT" },
            {"byte", "SMALLINT"},
            {"byte[]", "BYTEA"},
            {"guid", "UUID"},
            {"rowversion", "BYTEA"},
            {"xml", "XML"}
        });

      public PostgreSqlConnectionFactory(Connection connection) {
         _c = connection;
      }

      public IDbConnection GetConnection(string appName = null) {
         // TODO: switch from bcl date types to noda time, see https://www.npgsql.org/doc/types/datetime.html
         AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
         return new NpgsqlConnection(GetConnectionString(appName));
      }

      public string GetConnectionString(string appName = null) {
         if (_c.ConnectionString != string.Empty)
            return _c.ConnectionString;

         _c.ConnectionString = new NpgsqlConnectionStringBuilder {
            ApplicationName = appName ?? Constants.ApplicationName,
            Database = _c.Database,
            Host = _c.Server,
            Password = _c.Password,
            Username = _c.User,
            Port = _c.Port == 0 ? 5432 : _c.Port,
            Timeout = _c.RequestTimeout,
         }.ConnectionString;

         return _c.ConnectionString;
      }

      static char L { get; } = '"';
      static char R { get; } = '"';

      public bool SupportsLimit => true;

      /// <summary>
      /// The Postgres Server requires case sensativity if you enclose identifiers in double-quotes.  
      /// So, this is only done when necessary.
      /// </summary>
      /// <param name="name"></param>
      /// <returns></returns>
      public string Enclose(string name) {
         if (_c.Enclose) {
            return L + name + R;
         }
         return name.Contains(" ") || Reserved.Contains(name) ? L + name + R : name;
      }

      public string SqlDataType(Field f) {

         var length = (new[] { "string", "char" }).Any(t => t == f.Type) ? $"({(f.Length)})" : string.Empty;
         var dimensions = (new[] { "decimal" }).Any(s => s.Equals(f.Type)) ?
             $"({f.Precision},{f.Scale})" :
             string.Empty;

         var sqlDataType = Types[f.Type];

         var type = string.Concat(sqlDataType, length, dimensions);
         switch (type.ToLower()) {
            case "varchar(max)":
               return "TEXT";
            default:
               return type;
         }

      }

   }
}
