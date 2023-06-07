drop database library;
CREATE DATABASE library;
USE library;

drop user 'libraryuser'@'localhost';
flush privileges;

CREATE USER 'libraryuser'@'localhost' IDENTIFIED BY 'librarypass';
GRANT ALL on library.* to 'libraryuser'@'localhost';

-- Create user table
DROP TABLE IF EXISTS user;
CREATE TABLE user (
  id_user INT PRIMARY KEY AUTO_INCREMENT,
  fname VARCHAR(255),
  lname VARCHAR(255),
  username VARCHAR(255),
  password VARCHAR(255),
  address VARCHAR(255)
);

-- DO NOT INSERT AND USE NEW USERS IN SQL (use POST, because password needs to be hashed)
INSERT INTO user(id_user, fname, lname, username, password, address) VALUES(1, 'test', 'test', 'test', 'test', 'test');


-- Create book table
DROP TABLE IF EXISTS book;
CREATE TABLE book (
  id_book INT PRIMARY KEY AUTO_INCREMENT,
  title VARCHAR(255),
  author VARCHAR(255)
);

INSERT INTO book(title, author) VALUES('PHP Basic','Bob Jones');
INSERT INTO book(title, author) VALUES('Statistics','Lisa Smith');
INSERT INTO book(title, author) VALUES('C# Basics','Craig David');
INSERT INTO book(title, author) VALUES('Java for Dummies','John Smith');
INSERT INTO book(title, author) VALUES('SQL Advanced','Roger Allen');
INSERT INTO book(title, author) VALUES('Functional Maths','Jessica Power');
INSERT INTO book(title, author) VALUES('Robotics','Amy Winehouse');



-- Create review table
DROP TABLE IF EXISTS review;
CREATE TABLE review (
  id_review INT PRIMARY KEY AUTO_INCREMENT,
  review_date VARCHAR(30) NULL DEFAULT NULL,
  user_review INT,
  book_review INT,
  rating INT,
  FOREIGN KEY (user_review) REFERENCES user(id_user),
  FOREIGN KEY (book_review) REFERENCES book(id_book)
);

INSERT INTO review(id_review, review_date, user_review, book_review, rating) VALUES(1, NOW(), 1, 3, 10);
