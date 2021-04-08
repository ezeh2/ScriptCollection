create table GUEST.RANK_LIST_T
(
rank decimal(4,0) primary key,
name nvarchar(20),
time decimal(4,0)
);


insert into GUEST.RANK_LIST_T
(rank,name,time)
values(1,'John',110);

insert into GUEST.RANK_LIST_T
(rank,name,time)
values(2,'David',120);

insert into GUEST.RANK_LIST_T
(rank,name,time)
values(3,'Bob',121);

insert into GUEST.RANK_LIST_T
(rank,name,time)
values(4,'Hans',124);

insert into GUEST.RANK_LIST_T
(rank,name,time)
values(5,'Claus',126);

insert into GUEST.RANK_LIST_T
(rank,name,time)
values(6,'Gustav',140);


select 
r1.rank,r1.name,r1.time,
r2.rank,r2.name,r2.time,
r2.time-r1.time as time_diff
from GUEST.[RANK_LIST_T] r1
left join GUEST.[RANK_LIST_T] r2 on r1.rank+1=r2.rank
where r2.rank is not null
order by r2.time-r1.time
;
