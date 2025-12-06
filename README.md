# SafePass - Локальный менеджер паролей

Данный проект выполнен в рамках курсовой работы, студенткой Василенко Наталия из группы 241-333

Университет: Московский политехнический университет

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Windows](https://img.shields.io/badge/Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white)

##  Оглавление
- [Возможности](#-возможности)
- [Установка](#-установка)
- [Использование](#-использование)


##  Возможности

###  Основной функционал
- **Безопасное хранение** — все данные шифруются локально
- **CRUD операции** — создание, чтение, обновление и удаление записей
- **Мастер-пароль** — единый ключ для доступа к базе
- **Простой интерфейс** — интуитивно понятный Windows интерфейс основанный на Windows Forms


### Безопасность
- Локальное хранение данных (без облачных сервисов)
- Скрытый файл базы данных
- На данный момент используется шифрование AES-CBC
  
[!] - на данном этапе разработки не советуется хранить важную информацию в менеджере, только на свой страх и риск

[!] - в будущих обновлениях безопасность будет усиливаться, а этот список дополняться

[!] - когда приложение дойдет до этапа когда оно будет максимально безопасно - об этом будет написано в релизе

## Установка

### Скачивание готового приложения
1. Перейдите в раздел [Releases](https://github.com/aster-ix/SafePass_local-password-manager/releases)
2. Скачайте последнюю версию `SafePass_v1.0_winx64.zip`
3. Распакуйте архив в удобную папку
4. Запустите `SafePass_local_password-manager.exe`
#### приложение работает только на windows 10/11

### Сборка из исходного кода
```bash
# Клонирование репозитория
git clone https://github.com/aster-ix/SafePass_local-password-manager.git
cd SafePass_local-password-manager

# Сборка проекта
dotnet build -c Release

# Запуск приложения
cd SafePass_local_password-manager/bin/Release/net9.0-windows
./SafePass_local_password-manager.exe
