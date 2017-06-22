package main

//https://labix.org/mgo
import (
	"fmt"
	"log"
	"net/http"

	"time"

	"gopkg.in/mgo.v2"
	"gopkg.in/mgo.v2/bson"
)

type Person struct {
	Name      string    `json:"id"`
	Age       int       `json:"age,string"`
	Car       string    `json:"car,omitempty"`
	Country   string    `json:"country"`
	Email     string    `json:"email"`
	Timestamp time.Time `json:"-"`
}

func main() {

	http.HandleFunc("/insert", func(res http.ResponseWriter, req *http.Request) {

		session, err := mgo.Dial("localhost:27017")
		if err != nil {
			panic(err)
		}
		defer session.Close()

		// Optional. Switch the session to a monotonic behavior.
		session.SetMode(mgo.Monotonic, true)

		c := session.DB("test").C("people")
		index := mgo.Index{Key: []string{"email"}, Unique: true}
		err = c.EnsureIndex(index)
		if err != nil {
			panic(err)
		}
		err = c.Insert(&Person{"Laura", 34, "Audi", "Spain", "laura@gmail.com", time.Now()},
			&Person{"Natalia", 32, "Porsche", "Spain", "natalia@gmail.com", time.Now()},
			&Person{"Isabel", 32, "BMW", "Spain", "isabel@gmail.com", time.Now()},
			&Person{"Nuria", 40, "Ferrari", "Spain", "nuria@gmail.com", time.Now()})
		if err != nil {
			log.Fatal(err)
		}
	})

	http.HandleFunc("/get", func(res http.ResponseWriter, req *http.Request) {

		session, err := mgo.Dial("localhost:27017")
		if err != nil {
			panic(err)
		}
		defer session.Close()

		// Optional. Switch the session to a monotonic behavior.
		session.SetMode(mgo.Monotonic, true)

		c := session.DB("test").C("people")
		result := Person{}
		err = c.Find(bson.M{"name": "Isabel"}).One(&result)
		if err != nil {
			log.Fatal(err)
		}

		fmt.Println("Car: ", result.Car)
	})

	http.HandleFunc("/getall", func(res http.ResponseWriter, req *http.Request) {

		session, err := mgo.Dial("localhost:27017")
		if err != nil {
			panic(err)
		}
		defer session.Close()

		// Optional. Switch the session to a monotonic behavior.
		session.SetMode(mgo.Monotonic, true)

		c := session.DB("test").C("people")
		var result []Person
		err = c.Find(bson.M{"country": "Spain"}).Sort("-timestamp").All(&result)
		if err != nil {
			log.Fatal(err)
		}

		for _, v := range result {
			fmt.Println(v.Name)
		}
	})

	http.HandleFunc("/update", func(res http.ResponseWriter, req *http.Request) {

		session, err := mgo.Dial("localhost:27017")
		if err != nil {
			panic(err)
		}
		defer session.Close()

		// Optional. Switch the session to a monotonic behavior.
		session.SetMode(mgo.Monotonic, true)

		c := session.DB("test").C("people")
		err = c.Update(bson.M{"name": "Natalia"}, bson.M{"$set": bson.M{"car": "Lamborghini"}})
		if err != nil {
			log.Fatal(err)
		}
	})

	http.HandleFunc("/remove", func(res http.ResponseWriter, req *http.Request) {

		session, err := mgo.Dial("localhost:27017")
		if err != nil {
			panic(err)
		}
		defer session.Close()

		// Optional. Switch the session to a monotonic behavior.
		session.SetMode(mgo.Monotonic, true)

		c := session.DB("test").C("people")
		c.Remove(bson.M{"name": "Laura"})
	})

	http.HandleFunc("/count", func(res http.ResponseWriter, req *http.Request) {
		session, err := mgo.Dial("localhost:27017")
		if err != nil {
			panic(err)
		}
		defer session.Close()

		// Optional. Switch the session to a monotonic behavior.
		session.SetMode(mgo.Monotonic, true)

		c := session.DB("test").C("people")
		count, _ := c.Find(bson.M{"country": "Spain"}).Count()
		res.Write([]byte(string(count)))
	})

	http.ListenAndServe(":8080", nil)

}
