import { useState, useEffect } from 'react';
import './App.css';

function App() {

  const [todos, setTodos] = useState([{ name: "Loading..." }]);
  const [newTodo, setNewTodo] = useState({ id:0, name: "", isCompleted:false });

  const addNewTodo = (e) => {
    e.preventDefault();
    fetch("https://localhost:5001/api/todoitems",{
      method: "post",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(newTodo)
    }).then(response => response.json())
    .then(json => setTodos([...todos,json]));
    setNewTodo({id:0,name:"",isCompleted:false});
  }


  useEffect(() => {
    fetch('https://localhost:5001/api/todoitems')
      .then(response => response.json())
      .then(json => setTodos(json));
  }, []);

  return (
    <div className="App">
      <h1>My Todos</h1>
      <form onSubmit={addNewTodo}>
        <input type="text" value={newTodo.name}
         onInput={e=>setNewTodo({...newTodo,name:e.target.value })}
          placeholder="Write here." required/>

      </form>
      <ul>
        {todos.map((x, index) => <li key={index}>{x.name}</li>)}
      </ul>
    </div>
  )
}

export default App;
