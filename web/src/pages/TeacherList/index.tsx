import React, { useState, FormEvent, useEffect } from 'react';

import PageHeader from '../../components/PageHeader';
import TeacherItem, { Teacher } from '../../components/TeacherItem';
import Input from '../../components/Input';
import Select from '../../components/Select';

import './styles.css';
import api from '../../services/api';

function TeacherList() {
  const [fadeState, setFadeState] = useState("hidden");

  useEffect(() => {
    setFadeState("visible")
  }, []);

  const [teachers, setTeachers] = useState<Teacher[]>([]);

  const [subject, setSubject] = useState('');
  const [week_day, setWeekDay] = useState('');
  const [time, setTime] = useState('');

  async function searchTeachers(e: FormEvent) {
    e.preventDefault();

    const response = await api.get<Teacher[]>('classes', {
      params: {
        subject,
        week_day,
        time
      }
    });

    setTeachers(response.data);
  }


  return (
    <div id='page-teacher-list' className={`container ${fadeState}`}>
      <PageHeader title='Estes são os proffys disponíveis.'>
        <form id='search-teachers' onSubmit={searchTeachers}>
          <Select
            name='subject'
            label='Matéria'
            value={subject}
            onChange={e => setSubject(e.target.value)}
            options={[
              { value: 'Artes', label: 'Artes' },
              { value: 'Biologia', label: 'Biologia' },
              { value: 'Ciências', label: 'Ciências' },
              { value: 'Educação física', label: 'Educação física' },
              { value: 'Física', label: 'Física' },
              { value: 'Geografia', label: 'Geografia' },
              { value: 'História', label: 'História' },
              { value: 'Matemática', label: 'Matemática' },
              { value: 'Português', label: 'Português' },
              { value: 'Química', label: 'Química' }
            ]}
          />
          <Select
            name='week-day'
            label='Dia da semana'
            value={week_day}
            onChange={e => setWeekDay(e.target.value)}
            options={[
              { value: '0', label: 'Domingo' },
              { value: '1', label: 'Segunda-feira' },
              { value: '2', label: 'Terça-feira' },
              { value: '3', label: 'Quarta-feira' },
              { value: '4', label: 'Quinta-feira' },
              { value: '5', label: 'Sexta-feira' },
              { value: '6', label: 'Sábado' }
            ]}
          />

          <Input
            type='time'
            label='Hora'
            value={time}
            onChange={e => {
              setTime(e.target.value)
            }}
            name='time' />

          <button type="submit">
            Buscar
          </button>
        </form>
      </PageHeader>

      <main>
        {
          teachers.map((teacher) => {
            return <TeacherItem key={teacher.id} teacher={teacher} />
          })
        }
      </main>
    </div>
  );
}

export default TeacherList;