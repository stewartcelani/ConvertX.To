   FILE SECTION.
       FD  jr-file.
       01  jr-record.
           03  animal-info.
               05  patient-id                  pic x(5).
               05  atype                       pic x.
               05  ctype redefines atype       pic x.
               05  dtype redefines atype       pic x.
               05  otype redefines atype       pic x.
           03  owner-info.
               05  phone                       pic x(8).
               05  owner                       pic x(30). 
           03  financial.
               05  acct_no.
                   10  year                    pic x(2).
                   10  seq_no                  pic x(4).
               05  last_visit.
                   10  yyyy                    pic 9(4).
                   10  mm                      pic 9(2).
                   10  dd                      pic 9(2).
               05  fee                         pic s9(5)v99.
               05  date_paid                   pic 9(8).