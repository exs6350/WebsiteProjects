CREATE TABLE "Users" (
	"pId" character(36) NOT NULL,
	"Username" character varying(255) NOT NULL,
	"Email" character varying(128) NULL,
	"Comment" character varying(128) NULL,
	"Password" character varying(255) NOT NULL,
	"PasswordQuestion" character varying(255) NULL,
	"PasswordAnswer" character varying(255)	NULL,
	"IsApproved" boolean NULL, 
	"LastActivityDate" timestamptz NULL,
	"LastLoginDate" timestamptz NULL,
	"LastPasswordChangedDate" timestamptz NULL,
	"CreationDate" timestamptz NULL, 
	"IsOnLine" boolean NULL,
	"IsLockedOut" boolean NULL,
	"LastLockedOutDate" timestamptz NULL,
	"FailedPasswordAttemptCount" integer NULL,
	"FailedPasswordAttemptWindowStart" timestamptz NULL,
	"FailedPasswordAnswerAttemptCount" integer NULL,
	"FailedPasswordAnswerAttemptWindowStart" timestamptz NULL,
	"ProfileType" integer NOT NULL,
	CONSTRAINT users_pkey PRIMARY KEY ("pId"),
	CONSTRAINT users_username_unique UNIQUE ("Username")
);

CREATE INDEX users_profile_type_index ON "Users" ("ProfileType");

CREATE TABLE "Restaurants" (
	"RestaurantId" character(36) NOT NULL,
	"Name" character varying(100) NOT NULL,
	"FoodType" character varying(35) NULL,
	"City" character varying(36) NOT NULL,
	"State" character varying(36) NOT NULL,
	"Country" character varying(36) NOT NULL,
	"Location" point NULL,
	"Address" character varying(95) NULL,
	CONSTRAINT primID PRIMARY KEY ("RestaurantId")
);

CREATE TABLE "Reviews" (
	"ReviewText" text NOT NULL,
	"AverageRating" real NULL,
	"DatePosted" date NOT NULL,
	"UserId" character(36) NOT NULL,
	"ReviewId" character(36) NOT NULL,
	"RestaurantId" character(36) NOT NULL,
	CONSTRAINT UserId FOREIGN KEY ("UserId") REFERENCES Users ("pId") 
);

CREATE OR REPLACE FUNCTION search(query character varying) RETURNS refcursor AS
$BODY$DECLARE ref refcursor;
BEGIN
OPEN ref FOR SELECT * FROM "Restaurants" WHERE "Name" ILIKE '%' || query OR "Name" ILIKE query || '%'
OR "Name" ILIKE query ORDER BY "Name";
RETURN ref;
END;
$BODY$
  LANGUAGE plpgsql;