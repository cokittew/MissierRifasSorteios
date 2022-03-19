create database UserClienteDataContext;

use UserClienteDataContext;

-- User Interface and actions ----

create table user_basic(
id int identity, primary key(id),
id_identity varchar(10) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
email_verify bit default(0),
phone_verify bit default(0),
signature_active bit default(0),
user_number_bag int not null,
id_telegram nvarchar(50),
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

--alter table user_basic alter column id_telegram nvarchar(50)
--alter table user_basic add user_number_bag int null;
--alter table user_basic add id_identity varchar(10) COLLATE SQL_Latin1_General_CP1_CS_AS null;

create table user_basic_info(
id int identity, primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
full_name varchar(200) not null,
nick_name varchar(100) null,
email varchar(250) COLLATE SQL_Latin1_General_CP1_CS_AS unique not null,
phone varchar(11),
user_password varchar(25) COLLATE SQL_Latin1_General_CP1_CS_AS not null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

create table user_identity_info(
id int identity,primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
cpfCnpj varchar(14) unique not null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

create table user_guest(
id int identity, primary key(id),
id_telegram int unique,
cpfCnpj varchar(14) unique,
email varchar(250) COLLATE SQL_Latin1_General_CP1_CS_AS unique,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

create table user_email_confirmation(
id int identity,primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
code_access varchar(40) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

create table user_telegram_validation(
id int identity,primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
code_access varchar(8) COLLATE SQL_Latin1_General_CP1_CS_AS not null,
beginning_date dateTime default (GetDate()),
removed bit default(0));

create table user_social_media(
id int identity, primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
instagram varchar(100) null,
twitter varchar(100) null,
youtube varchar(250) null,
facebook varchar(250) null,
reddit varchar(250) null,
kwai varchar(250) null,
whatsapp varchar(25) null,
tikTok varchar(250) null,
another_informations text null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

--alter table user_social_media add whatsapp varchar(25) null
--drop table user_social_media;

create table pix_key_types(
id int identity, primary key(id),
name_type varchar(35) not null unique,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

create table user_pix_information(
id int identity, primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
pix_key_type int, foreign key(pix_key_type) references pix_key_types(id),
pix_key varchar(100),
what_services_use varchar(200) null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

create table user_bank_information(
id int identity, primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
what_services_use varchar(200) null,
bank_code varchar(5) not null,
bank_account varchar(15) not null,
agence_account varchar(5) not null,
cpf_owner varchar(11) not null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

--drop table user_bank_information;

------------- end ----------

----------User Platforms

--Raffle
create table platform_all_services_basic(
id int identity, primary key(id),
name_service varchar(200) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
description_service text,
is_free bit default(0),
is_limited bit default(1),
signature_type int default(2),
beginning_date dateTime default(GetDate()),
removed bit default(0)
);

create table platform_service_raffle(
id int identity, primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
id_platform_service int not null, foreign key(id_platform_service) references platform_all_services_basic(id),
id_identity varchar(10) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
raffle_name varchar(200) COLLATE SQL_Latin1_General_CP1_CS_AS not null,
raffle_general_description varchar(600) not null,
raffle_premiation_description varchar(500) not null,
raffle_max_number int not null,
raffle_payment_id_allowed text not null,
raffle_user_max_numbers int not null,
raffle_number_value decimal(10,2) not null,
raffle_status int not null,
raffle_start_date date,
raffle_end_date date,
raffle_winners_number int not null default(1),
raffle_numbers_result varchar(250),
beginning_date dateTime default (GetDate()),
raffle_close_option bit default(0),
removed bit default(0)
);

--alter table platform_service_raffle add raffle_winners_number int not null default(1)

--alter table platform_service_raffle add raffle_numbers_result varchar(250);
--alter table platform_service_raffle add raffle_close_option bit default(0);

create table platform_service_raffle_informations(
id int identity, primary key(id),
id_raffle int not null, foreign key(id_raffle) references platform_service_raffle(id),
raffle_easy_link varchar(250) COLLATE SQL_Latin1_General_CP1_CS_AS unique,
raffle_link_result text COLLATE SQL_Latin1_General_CP1_CS_AS null,
raffle_participant text default('None'),
--raffle_numbers_result varchar(250),
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

--alter table platform_service_raffle_informations drop column raffle_numbers_result

create table platform_guest_reserved_number(
id int identity, primary key(id),
id_raffle int not null, foreign key(id_raffle) references platform_service_raffle(id),
id_identity varchar(10) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
full_name varchar(200) not null,
number int not null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

create table platform_user_reserved_number(
id int identity, primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
id_raffle int not null, foreign key(id_raffle) references platform_service_raffle(id),
number int not null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

create table platform_payment_transactions(
id int identity, primary key(id),
id_raffle int null,
id_basic_user int not null,
reference varchar(25) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
final_status varchar(30) not null,
number_quantity int not null,
typeTransaction int not null default(1),
totalValue decimal(14,2) not NULL default(0),
referenceId varchar(250) not null default(''),
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

alter table platform_payment_transactions add referenceId varchar(250) not null default('');
alter table platform_payment_transactions add typeTransaction int not null default(1);
alter table platform_payment_transactions add [id_raffle] [int] NULL;
alter table platform_payment_transactions add totalValue decimal(14,2) not NULL default(0);

drop table platform_payment_transactions;

create table platform_service_raffle_payment_types_allowed(
id int identity, primary key(id),
id_raffle int not null, foreign key(id_raffle) references platform_service_raffle(id),

beginning_date dateTime default (GetDate()),
removed bit default(0)
);

create table platform_service_raffle_payment_information(
id int identity, primary key(id),
raffle_mercadopago_public_key varchar(42),
raffle_mercadopago_access_token varchar(200),

beginning_date dateTime default (GetDate()),
removed bit default(0)
)

------------- end ----------

create table platform_service_raffle_files (
id int identity, primary key(id),
id_raffle int not null, foreign key(id_raffle) references platform_service_raffle(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
index_type varchar(100) not null,
number_sequence text not null,
receipt_value decimal(10,2) not null,
receipt_file nvarchar(max) not null,
pre_aproved bit default(1),
aproved bit default(0),
removed bit default(0),
beginning_date dateTime default (GetDate())
);

drop table platform_service_raffle_files;

select * from platform_service_raffle_files;

alter table platform_service_raffle_files add pre_aproved bit default(1);
update platform_service_raffle_files set aproved = 0;

create table raffle_business_participant(
id int identity, primary key(id),
id_raffle int not null, foreign key(id_raffle) references raffle_business_raffle(id),
id_identity varchar(10) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
full_name varchar(200) not null,
email varchar(250) COLLATE SQL_Latin1_General_CP1_CS_AS not null,
phone varchar(11),
numbers text not null,
participant_status int not null default(1),
collaborator_code varchar(8) null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

alter table raffle_business_participant add collaborator_code varchar(8) null;

create table raffle_business_raffle(
id int identity, primary key(id),
id_basic_user int not null, foreign key(id_basic_user) references user_basic(id),
id_identity varchar(10) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
raffle_name varchar(200) COLLATE SQL_Latin1_General_CP1_CS_AS not null,
raffle_general_description varchar(600) not null,
raffle_premiation_description varchar(500) not null,
raffle_number_value decimal(10,2) not null,
raffle_start_date date,
raffle_end_date date,
raffle_status int not null default(1),
raffle_receipt_file nvarchar(max) null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

alter table raffle_business_raffle add raffle_receipt_file nvarchar(max) null;

create table raffle_business_collaborator(
id int identity, primary key(id),
id_identity varchar(10) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
pass_word varchar(20) not null,
personal_code varchar(8) COLLATE SQL_Latin1_General_CP1_CS_AS not null unique,
phone varchar(11),
full_name varchar(200) not null,
pix_type varchar(50),
pix_key varchar(100),
your_cash decimal(10,2) not null,
your_cash_percentage decimal(10,2) not null,
email varchar(250) COLLATE SQL_Latin1_General_CP1_CS_AS not null,
beginning_date dateTime default (GetDate()),
removed bit default(0)
);

--drop table raffle_business_collaborator;

create table misser_worker(
id int identity, primary key(id),
full_name varchar(200) not null,
email varchar(250) COLLATE SQL_Latin1_General_CP1_CS_AS not null,
pass_word varchar(20) not null,
beginning_date dateTime default (GetDate()),
hasPermission bit default(0),
removed bit default(0)
);

create table collaborator_payment_register(
id int identity, primary key(id),
collaborator_id int, foreign key(collaborator_id) references raffle_business_collaborator(id),
missier_worker_id int, foreign key(missier_worker_id) references misser_worker(id),
period_value decimal(14,2) not null,
period_time date not null,
receipt_file nvarchar(max) null,
observation text null,
payment_date datetime,
is_payed bit default(0),
removed bit default(0),
beginning_date dateTime default (GetDate()),
);